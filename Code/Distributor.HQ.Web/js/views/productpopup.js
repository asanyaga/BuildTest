define([
        'jquery',
        'underscore',
        'backbone',
        'jqueryui',
        'apputil',
        'text!templates/productpopup_template.html',
'accounting'

], function ($, _, backbone, jqueryui, apputil, casecodetemplate, accounting) {

    // user form view

    var Model = Backbone.Model.extend({

        defaults: {
            Product: "",
            PackageOption: 'false',
            Quantity:1,
            NetAmount: 0,
            UnitPrice: 0,
            GrossAmount: 0,
            IsEdit:false,
           
        },
        validate: function (attrs, options) {
            var errors = [];
            if (!attrs.Name) {
                errors.push({ name: 'name', message: 'Please fill name field.' });
            }
            return errors.length > 0 ? errors : false;
        },
        url: window.app_baseurl + "api/client/counterpartyclass/save",

    });

    var formView = Backbone.View.extend({
        initialize: function (option) {
            this.$el = $('#modal-productpopup');
            this.cid = "view_productpopup_form";
            this.template = _.template(casecodetemplate);
            this.model = new Model();
            this.Products = new Array();
            debugger;
            apputil.quantity_filter = new RegExp(option.QuantityRegex);
            if (option != null && option.model != null) {
                this.model = option.model;
                var productId = option.model.get("ProductId");
                var productname = option.model.get("Product");
                this.Products.push({ Value: productId, Text: productname });
                this.calculatesummary();
            }
         
            var self = this;
            this.model.on('change', function () {
                apputil.hideErrors(self);
            });
            this.model.on('change:ProductId', function() {
                self.calculatesummary();
            });
            this.model.on('change:Quantity', function () {
                self.calculatesummary();
            });
            this.model.on('change:PackageOption', function () {
                self.calculatesummary();
            });


        },
        events: {
            'refreshitem': 'refresh',
            'click #product': 'selectproduct',
            

        },
        validate: function (attrs, options) {
           
            var errors = [];
            if (!attrs.ProductId) {
                errors.push({ name: 'product', message: 'Please select product' });
            }
            if (!apputil.quantity_filter.test(attrs.Quantity)) {
                errors.push({ name: 'quantity', message: 'Please enter valid quantity  .' });
            }
           
            return errors.length > 0 ? errors : false;
        },
        calculatesummary: function () {
            this.model.validate = this.validate;
            var self = this;
            if (!self.model.isValid()) {
                apputil.showErrorsAlert(self.model.validationError);
                return;
            }
          
            var productId = this.model.get("ProductId");
            var quantity = this.model.get("Quantity");
            var packageOption = this.model.get("PackageOption");
            $.get(window.app_baseurl + "api/CalculatePurchaseOrderItemSummary?productId=" + productId + "&quantity=" + quantity + "&isBulk="+packageOption, {},
                function (data) {
                   
                    var unit = 0;
                    var netamount = 0;
                    var grossamount = 0;
                    _(data).each(function (dv) {
                      
                        unit += dv.Product.ExFactoryPrice;
                        netamount += dv.Product.ExFactoryPrice * dv.Quantity;
                        grossamount += dv.Product.ExFactoryPrice * dv.Quantity;
                    });
                    self.model.set({ UnitPrice: unit });
                   
                    self.model.set({ NetAmount: netamount, GrossAmount: grossamount });
                    
                }
            );
       
        },
        selectproduct: function (event) {

        event.preventDefault();
        var popupclass = apputil.DropdownPopupView;
        var popup = new popupclass(
            {
                placeholder: "#popup-dropdown",
                collectionname: "SaleProducts",
                label: "List of Product"
            });
        this.listenTo(popup, 'itemselected', this.productselected);
        popup.render();
           
    },
        productselected: function (option) {
       
        var id = option.model.get("Value");
        var name = option.model.get("Text");
        this.Products = [];
        this.Products.push({ Value: id, Text: name });
        this.model.set({ ProductId: id, Product: name });
        this.stickit();

    },
        bindings: {
            '#unitprice': {
                observe: 'UnitPrice',
                updateMethod: 'html',
                escape: true,
                onGet: function (val) {
                    return accounting.formatMoney(val, "", 2, ",", ".");
                }
            },
            '#netamount': {
                observe: 'NetAmount',
                updateMethod: 'html',
                escape: true,
                onGet: function (val) {
                    return accounting.formatMoney(val, "", 2, ",", ".");
                }
            },
            '#grossamount': {
                observe: 'GrossAmount',
                updateMethod: 'html',
                escape: true,
                onGet: function (val) {
                    return accounting.formatMoney(val, "", 2, ",", ".");
                }

            },
            '#quantity': 'Quantity',
            "#packageoption": 'PackageOption',
            'select#product': {
                observe: 'ProductId',
                selectOptions: {
                    collection: function () {
                        return this.Products;
                    },
                    labelPath: 'Text',
                    valuePath: 'Value'
                }
            },
        },


        render: function () {

            var self = this;
            self.model.validate = self.validate;
            this.$el.html(this.template()).dialog({
                resizable: false,
                height: 430,
                width: 500,
                modal: true,
                title: 'Product Picker',
                buttons: {
                    "Add": function () {
                        
                        if (!self.model.isValid()) {
                            apputil.showErrorsAlert(self.model.validationError);
                            return;
                        }
                        $(this).dialog("close");
                        self.trigger('refreshitem', self.model);
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                    }
                }
            });
            this.stickit();
            return this;
        },

    });
    return formView;
});