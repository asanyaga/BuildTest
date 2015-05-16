define([
        'jquery',
        'underscore',
        'backbone',
        'jqueryui',
        'apputil',
    'accounting',
     'backbone-stickit',
    'productpopupviewmodel',
    'moment',
        'text!templates/addupdate_purchaserorder_template.html',
        'toastr'
], function ($, _, backbone, jqueryui, apputil, accounting, stickit, ProductPopup, moment, template, toastr) {

    
    var Model = Backbone.Model.extend({
        defaults: {
            RequiredDate: moment().format("DD-MMM-YYYY"),
            headerName: "test",
            DistributorId: "",
            Items: [],
            TotalNetAmount: 0,
            TotalGrossAmount: 0,
            Status:"NEW"
        }
    });
    var formView = Backbone.View.extend({
        initialize: function (option) {
            this.$el = $('#pagecontentholder');
            this.cid = "view_addupdateformitem";
            this.template = _.template(template);
            this.model = new Model();
            this.Distributors = new Array();
           // alert("test 3");
            //var date=moment().format("DD-MMM-YYYY");
            //this.model.set({ RequiredDate: date });
            
        },
        events: {
            'click #btn-add-item': 'add',
            'click #distributorid': 'selectdistributor',
            'click .btn-editrow': 'edit',
            'click #btn-confirm-order': 'confirmorder',
            'click .btn-deleterow': 'deleteitem',
            'click #btn-cancel': 'cancel',

        },
        cancel: function (event) {
            event.preventDefault();
            debugger;
            var r = confirm("Are you sure you want to cancel this order ?");
            if (r == false)
                return;
            this.model = new Model();
            var newKey = apputil.Guid();
            $("#cache-key").val(newKey);
            this.stickit();
            toastr.success('Purchase order canceled successfully');
            
        },
        selectdistributor: function (event) {

            event.preventDefault();
            var popupclass = apputil.DropdownPopupView;
            var popup = new popupclass(
                {
                    placeholder: "#popup-dropdown",
                    collectionname: "Distributors",
                    label: "List of Distributors"
                });
            this.listenTo(popup, 'itemselected', this.distributorchanged);
            popup.render();
           
        },
        distributorchanged: function (option) {
            
            var id = option.model.get("Value");
            var name = option.model.get("Text");
            this.Distributors = [];
            this.Distributors.push({ Value: id, Text: name });
            this.model.set({ DistributorId: id });
            this.stickit();

        },
        deleteitem: function (event) {
            event.preventDefault();
            
            var r = confirm("Are you sure you want to remove this item ?");
            if (r == false)
                return;
            var self = this;;
            var productId  = $(event.currentTarget).data('id');
            var items = [];

            var cachekey = $("#cache-key").val();
            $.get(window.app_baseurl + "api/DeletePurchaseOrderItem?productId=" + productId  + "&key=" + cachekey , {},
                function (data) {

                    var unit = 0;
                    var netamount = 0;
                    var grossamount = 0;
                    _(data).each(function (dv) {

                        unit += dv.Product.ExFactoryPrice;
                        netamount += dv.Product.ExFactoryPrice * dv.Quantity;
                        grossamount += dv.Product.ExFactoryPrice * dv.Quantity;
                        items.push({
                            ProductId: dv.Product.Id,
                            Product: dv.Product.Description,
                            Quantity: dv.Quantity,
                            UnitPrice: dv.Product.ExFactoryPrice,
                            NetAmount: dv.Product.ExFactoryPrice * dv.Quantity,
                            GrossAmount: dv.Product.ExFactoryPrice * dv.Quantity,
                            IsEditable: dv.IsEditable
                        });
                    });

                    self.model.set({
                        Items: items,
                        TotalNetAmount: netamount,
                        TotalGrossAmount: grossamount
                    });
                    self.stickit();
                    toastr.success('Item removed successfully');
                });

           
        },
        add: function (event) {
            event.preventDefault();
           
            var quantityRegex = $("#quantityRegex").val();
           
            var popup = new ProductPopup({ QuantityRegex: quantityRegex });
            this.listenTo(popup, 'refreshitem', this.reload);
            popup.render();
        },
        edit: function (event) {
            event.preventDefault();
            var quantityRegex = $("#quantityRegex").val();
            var id = $(event.currentTarget).data('id');
            var name ="";
            _(this.model.get("Items")).find(function (dv) {
               
                if(dv.ProductId==id) {
                    name = dv.Product;
                    return;
                }
            });
            var quantity = $(event.currentTarget).data('quantity');
            var model = Backbone.Model.extend({
                defaults: {
                    Product: name,
                    ProductId:id,
                    PackageOption: 'false',
                    Quantity: quantity,
                    IsEdit:true,
                },
            });
            var popup = new ProductPopup({ QuantityRegex: quantityRegex, model: new model() });
            this.listenTo(popup, 'refreshitem', this.reload);
            popup.render();
        },
        confirmorder: function (event) {
            event.preventDefault();
            debugger;
            this.model.validate = this.validate;
            var self = this;
            if (!self.model.isValid()) {
                apputil.showErrorsAlert(self.model.validationError);
                return;
            }
            self.model.url = window.app_baseurl + "api/SavePurchaseOrder";
            self.model.sync("create", self.model, {
                success: function (model, response) {
                    debugger;
                    if (model.Result == "Fail") {
                        alert(model.ErrorInfo);
                        return;
                    } else {
                        
                        toastr.success(model.ResultInfo, 'Purchase order');
                        alert(model.ResultInfo);
                        window.location.replace(window.app_baseurl + "Admin/PurchaseOrder");
                    }
                },
                error: function (model, response) {

                    alert("fail");
                }
            });
        },
        validate: function (attrs, options) {

            var errors = [];
            if (!attrs.DistributorId) {
                errors.push({ name: 'DistributorId', message: 'Please select Distributor' });
            }
            if (!apputil.date_filter.test(attrs.RequiredDate)) {
                errors.push({ name: 'RequiredDate', message: 'Please enter valid Required date  .' });
            }
            if (attrs.Items.length == 0) {
                errors.push({ name: 'Items', message: 'Make sure you have add atleast one item.' });
            }

            return errors.length > 0 ? errors : false;
        },
        reload: function (option) {
            var self = this;
           
            var isedit=  option.get("IsEdit");
            var productId = option.get("ProductId");
            var quantity = option.get("Quantity");
            var packageOption = option.get("PackageOption");
            var items = [];
            
          
            var cachekey = $("#cache-key").val();
            $.get(window.app_baseurl + "api/CalculatePurchaseOrderItemFullSummary?productId=" + productId + "&quantity=" + quantity + "&isBulk=" + packageOption + "&key=" + cachekey + "&isEdit="+isedit, {},
                function (data) {
                    
                    var unit = 0;
                    var netamount = 0;
                    var grossamount = 0;
                    _(data).each(function (dv) {
                       
                        unit += dv.Product.ExFactoryPrice;
                        netamount += dv.Product.ExFactoryPrice * dv.Quantity;
                        grossamount += dv.Product.ExFactoryPrice * dv.Quantity;
                        items.push({
                            ProductId: dv.Product.Id,
                            Product: dv.Product.Description,
                            Quantity: dv.Quantity,
                            UnitPrice: dv.Product.ExFactoryPrice,
                            NetAmount: dv.Product.ExFactoryPrice * dv.Quantity,
                            GrossAmount: dv.Product.ExFactoryPrice * dv.Quantity,
                            IsEditable: dv.IsEditable
                        });
                    });

                    self.model.set({
                        Items: items,
                        TotalNetAmount: netamount,
                        TotalGrossAmount: grossamount
                    });
                    self.stickit();
                    if(isedit)
                        toastr.success('Item editted successfully');
                    else
                        toastr.success('Item added successfully');
                        
                });
           
            
            
           // items = this.model.get("Items");
           
        },
        bindings: {
            '#requireddate': 'RequiredDate',
            '#totalnetamount': {
                observe: 'TotalNetAmount',
                updateMethod: 'html',
                escape: true,
                onGet: function (val) {
                    return accounting.formatMoney(val, "", 2, ",", ".");
                }
            },
            '#status': {
                observe: 'Status',
                updateMethod: 'html',
                escape: true
            },
            '#totalgrossamount': {
                observe: 'TotalGrossAmount',
                updateMethod: 'html',
                escape: true,
                onGet: function (val) {
                   
                    return accounting.formatMoney(val, "", 2, ",", ".");
                }
            },
            'select#distributorid': {
                observe: 'DistributorId',
                selectOptions: {
                    collection: function () {
                        return this.Distributors;
                    },
                    labelPath: 'Text',
                    valuePath: 'Value'
                }
            },
            '#order-items': {
                observe: 'Items',
                updateMethod: 'html',
                onGet: function (val) {
                  
                  
                    var html = '<table width="100%" id="tablesorter-demo" class="tablesorter">';
                    html += '<thead>' +
                        '<tr><th class="header">Product</th>' +
                        '<th class="header">Quantity</th>' +
                        '<th class="header">Unit Price</th>' +
                        '<th class="header">Net Amount</th>' +
                        '<th class="header">Gross Amount</th>' +
                         '<th class="header"></th>' +
                         '<th class="header"></th></tr>' +
                        '</thead><tbody>';
                    _(val).each(function (dv) {

                        html += "<tr>" +
                            "<td> " + dv.Product + "</td>" +
                            "<td style='text-align: right'>" + accounting.formatNumber(dv.Quantity, 2, " ") + " </td>" +
                            "<td style='text-align: right'>" + accounting.formatMoney(dv.UnitPrice, "", 2, ",", ".") + " </td>" +
                            "<td style='text-align: right'>" + accounting.formatMoney(dv.NetAmount, "", 2, ",", ".") + " </td>" +
                            "<td style='text-align: right'>" + accounting.formatMoney(dv.GrossAmount, "", 2, ",", ".") + " </td>";
                        if(dv.IsEditable) {
                            html += '<td><a href="#" data-id=' + dv.ProductId + ' data-quantity=' + dv.Quantity + ' class="btn-editrow" >Edit </a> </td>';
                            html += '<td><a href="#" data-id=' + dv.ProductId + ' class="btn-deleterow" >Remove </a> </td>';

                        }else {
                            html += '<td> </td><td> </td>';
                        }
                            
                           html +=  "</tr>";
                    });
                    html += "</table>";
                    return html;
                }
            }
        },


        render: function () {
            var self = this;
            this.$el.html(this.template);
          
            self.$el.find("#requireddate").datepicker({
                dateFormat: 'dd-M-yy',
                gotoCurrent: true,
                showStatus: true,
                highlightWeek: true,
                showAnim: 'scale',
                firstDay: 6
            });
            this.stickit();
            return this;
        },

    });
    return formView;
});