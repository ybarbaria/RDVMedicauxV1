/// <reference path="../../../Libs/_references.js" />
/***************************************************
*
* @class activity.view.result
* Module gestion liste résultat activités.
* @uses jquery
* @uses ko
* @uses bootstrap
* @uses common.ajaxManager
* @uses data.refSvc
*
****************************************************/
define([
    'jquery',
    'ko',
    'bootstrap',
    'amplify',
    'common/ajaxManager',
    'data/refSvc'
],
    function ($, ko, bs, amplify, ajaxMgr, uiHelper, refSvc) {
        "use strict";
        var
           /**
           * @property {Object} _module
           * @private
           * Le module courant: index
           */
           _module,

             // return _module
        _module = {

            /**
            * @property {Object} vue modèle
            * xxxxxxxxxx
            */
            vm: {
                // Listes des villes
                villesList: ko.observableArray()
            },

            /**
            * Initialisation niveau DOM (Combo, DatePicker...)
            * @method render
            */
            render: function () {
                // chargement combo etat
                //refSvc.loadCombo(refSvc.ref.villes, _module.vm.villesList);
            },

            /**
           * Initialisation du module
           * @method initialize
           */
            initialize: function () {
                ko.applyBindings(_module.vm, $('#annuaire')[0]);
            },


        };

        // AUTO INIT
        _module.initialize();

        return _module;
    });

