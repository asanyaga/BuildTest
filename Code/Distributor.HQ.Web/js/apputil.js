define([
        'jquery',
        'underscore',
        'md5',
        'backbone',
        'backbone-pageable',
        'backgrid',
        'backgrid-paginator',
        'jqueryui',
        'backgrid-filter',
        'text!templates/popuplist_template.html',
        'magnific-popup'
], function ($, _, Md5, backbone, backbonepageable, backgrid, backgridpaginator, jqueryui, backgridfilter, dropdownpopuptemplate,magnificPopup) {
    Backbone.PageableCollection = backbonepageable;
    
    var guid = function() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    },
        showErrors = function(view, errors) {
           
            _.each(errors, function(error) {
               
                var controlGroup = view.$('.' + error.name);
                controlGroup.addClass('has-error');
                controlGroup.find('.help-block').text(error.message);
            }, this);
        },
        hideErrors = function(view) {
            view.$('.col-lg-8').removeClass('has-error');
            view.$('.help-block').text('');
        },
        showErrorsAlert = function (errors) {
            var msg = "";
            _.each(errors, function (error) {
                msg += error.message+"\n";
            }, this);
            alert(msg);
        }, model = Backbone.Model.extend({
            idAttribute: "Id",
            defaults: {
                Name: "",
                Code: ""
            }
        }),
        collection = Backbone.PageableCollection.extend({
            model: model,
            initialize: function (options) {
                this.cname = options.collectionname;
            },
            url: function () {
                var uri = window.app_baseurl;
                switch (this.cname) {
                    case "Distributors":
                        uri += 'api/GetDistributor';
                        break;
                    case "SaleProducts":
                        uri += 'api/GetSaleProducts';
                        break;
                        
                }
                return uri;
            },
            state: {
                pageSize: 10,
                firstPage: 1,
            },
            parse: function (response) {
                var tags = response.Data;
                var newState = _.clone(this.state);
                newState.totalRecords = response.RecordCount;
                this.state = this._checkState(newState);
                return tags;
            },
        }),
        dropdownpopupView = Backbone.View.extend({
            events: {
                'itemselected': 'itemselected',
            },
            initialize: function (option) {
                this.$el = $(option.placeholder);
                this.cid = "View_DropdownList";
                this.template = _.template(dropdownpopuptemplate);
                this.collectionname = option.collectionname;
                this.label = "Dropdown List";
              
                if (option.label != null) {
                    this.label = option.label;
                }

                this.pagedcollection = new collection(
                    {
                        collectionname: this.collectionname
                    });
            },
            render: function () {
                var self = this;
                this.$el.html(this.template).dialog({
                    dialogClass: 'noTitleStuff' ,
                    resizable: false,
                    height: 500,
                    width: 500,
                    modal: true,
                    title: self.label,
                    create: function (event, ui) {
                        // self.$el.siblings('div.ui-dialog-titlebar').remove();
                    },
                    buttons: {
                        "Cancel": function () {
                            $(this).dialog("close");

                            //self.close();
                        }
                    }
                });
                $(".ui-dialog-titlebar").hide();
                
            
                //this.$el.html(this.template).magnificPopup({
                //    items: {
                //        src: "#list-toolbar",
                //        type: 'inline'
                //    }
                //    // other options
                //});

                this.showView();

                return this;
            },
            selectedRow: function (option) {
                var self = this;
                self.trigger('itemselected', option);
                this.$el.dialog("close");

                //self.close();


            },
            showView: function () {

                var self = this;
                var grid = self.setupGrid().grid;
                self.$el.find("#grid").html(grid.render().$el);
                self.$el.find("#grid-paginator").html(self.setupGrid().paginator.render().$el);
                var filter = self.setupGrid().filter.render().$el;
                filter.find('div').removeClass('input-append').removeClass('input-prepend');
                self.$el.find("#grid-filter").html(filter);
                self.pagedcollection.fetch({ reset: true });
            },
            setupGrid: function () {
                var self = this;
                var columns = [
                    { name: "Text", label: "Item", cell: "string", editable: false, },
                    //{ name: "Code", label: "Code", cell: "string", editable: false, },
                    {
                        name: '',
                        label: 'Action',
                        cell: Backgrid.Cell.extend({
                            template: _.template('<a href="#" class="btn btn-mini selectrow" >Select</a>'),
                            events: {
                                "click .selectrow": "selectrow",
                            },
                            selectrow: function (e) {

                                e.preventDefault();
                                self.selectedRow({ model: this.model });
                            },

                            render: function () {
                                this.$el.html(this.template());
                                this.delegateEvents();
                                return this;
                            }
                        })
                    }
                ];
                var serverSideFilter = new backgrid.Extension.ServerSideFilter({
                    collection: self.pagedcollection,
                    name: "search",
                    placeholder: "Search ...",
                });

                serverSideFilter.$el.css({ float: "right" });

                var paginator = new backgrid.Extension.Paginator({
                    collection: self.pagedcollection
                });
                var grid = new backgrid.Grid({
                    columns: columns,
                    collection: self.pagedcollection
                });
                return {
                    grid: grid,
                    paginator: paginator,
                    filter: serverSideFilter
                };
            }
        });
    
        
        
    
        return {
            Guid: guid,
            showErrors: showErrors,
            hideErrors: hideErrors,
            showErrorsAlert:showErrorsAlert,
            date_filter: /^(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}$/,//dd-MMM-yyyy
            email_filter: /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/,
            phone_filter: /^(\d{4}-\d{3}-\d{3})+$/,
            time_filter: /^([01]?[0-9]|2[0-3]):[0-5][0-9]$/,
            number_filter: /^\d{1,4}$/,
            quantity_filter: /^(?=.*[0-9])(\d{0,10})?(?:\.\d{0,2})?$/,
            DropdownPopupView: dropdownpopupView,
           
        };

    });