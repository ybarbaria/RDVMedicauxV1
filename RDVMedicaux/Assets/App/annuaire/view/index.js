/// <reference path="../../../Libs/_references.js" />
/***************************************************
*
* @class annuaire.view.index
*
****************************************************/
define([
    'jquery',
    'ko',
    'bootstrap',
    'common/ajaxManager',
    'common/notify'
],
    function ($, ko, bs, ajaxMgr, notify) {
        "use strict";
        var
        /**
        * @property {Object} _module
        * @private
        * Le module courant: index
        */
        _module,
        _rootApi = APP.baseUrl;

        /**
        * Lancement de la recherche
        * @method search
        * @private
        */
        function _search() {
            // dans tout les cas on vide les données
            _module.vm._removeData();

            ajaxMgr.postJson(
                   _rootApi + 'Api/AnnuaireApi/Filter',
				   _module.vm.toJS()
               ).done(
               function (msg) {
                   if (msg.success) {
                       if (msg.data) {
                           // ajout des practiciens
                           _module.vm._loadData(msg.data);
                           notify.success(msg.data.length + " Practiciens trouvés");
                       }
                       else {
                           // pas de résultats
                           notify.error("Aucun practicien trouvé");
                       }

                   } else {

                       // Erreur interne 
                       notify.error("Erreur lors de la recherche");
                   }
               });
        }

        /**
        * Constructeur de la view model Annuaire
        * @method AnnuaireViewModel
        * @private
        */
        function AnnuaireViewModel() {
            var me = this;

            // Listes des specialités
            this.listSpecialite = ko.observableArray();
            this.selectedSpec = ko.observable();

            // Lieu saisi
            this.lieu = ko.observable();

            // Position GPS
            this.locLat = 0;
            this.locLong = 0;

            // Checkbox de tri
            this.sortByName = ko.observable();

            // Lors du click on tri automatiquement les résultats
            this.sortByName.subscribe(function () {
                me.listPracticien.sort(function (a, b) {
                    if (a.distance != 0 && b.distance != 0)
                    {
                        return (a.nom.localeCompare(b.nom) && a.distance > b.distance);
                    } else {
                        return a.nom.localeCompare(b.nom);
                    }
                });
            });

            this.sortBySector1 = ko.observable();

            // Recherche de practiciens
            this._onSearch = _search;

            // Permet de récuperer les données de la vue au format JSON
            this.toJS = function () {
                var data = ko.toJS(this);
                return data;
            };

            // List des practiciens
            this.listPracticien = ko.observableArray();

            // Load des données lors du retour serveur
            this._loadData = function (data) {
                me.listPracticien(data);
            }

            // Vide les données de la liste des practiciens
            this._removeData = function () {
                me.listPracticien.removeAll();
            }

            this._orderByName = function (state) {
                me.listPracticien.sort(function (a, b) {
                    return a.nom.localeCompare(b.nom);
                });
            }

            // Ajout d'un binding pour la gestion de l'api google
            ko.bindingHandlers.addressAutocomplete = {
                init: function (element, valueAccessor, allBindingsAccessor) {
                    var value = valueAccessor(), allBindings = allBindingsAccessor();

                    var options = {
                        types: ['geocode'],
                        componentRestrictions: {
                            country: "fr"
                        }
                    };
                    ko.utils.extend(options, allBindings.autocompleteOptions)

                    var autocomplete = new google.maps.places.Autocomplete(element, options);

                    google.maps.event.addListener(autocomplete, 'place_changed', function () {
                        var result = autocomplete.getPlace();

                        // Récupération de l'adresse courte
                        value(result.vicinity);

                        // Récupération de la position GPS
                        me.locLat = result.geometry.location.A;
                        me.locLong = result.geometry.location.F;
                    });
                },
                update: function (element, valueAccessor, allBindingsAccessor) {
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            };
        }

        // return _module
        _module = {

            /**
            * @property {Object} vue modèle
            * View Annuaire
            */
            vm: null,

            /**
            * Initialisation niveau DOM 
            * @method render
            */
            render: function () {
                var listSpec = [
                    { id: 10, label: "Dermatologue" },
                    { id: 21, label: "Ophtalmologue" },
                    { id: 22, label: "ORL" },
                ];

                this.vm = new AnnuaireViewModel();

                this.vm.listSpecialite(listSpec);

                ko.applyBindings(_module.vm, $('#annuaire')[0]);
            },

            /**
            * Initialisation du module
            * @method initialize
            */
            initialize: function () {

            },
        };

        // AUTO INIT
        _module.initialize();

        return _module;
    });

