/// <reference path="../../Libs/_references.js" />
/*******************************************************************************************
*
* @class annuaire.main
*
*********************************************************************************************/
require([
    'domready!',
    'data/cfgSvc',
    'annuaire/view/index'
],
    function (document, cfgSvc, indexView) {
        "use strict";
        /**
        * @method initialize
        */
        cfgSvc.initialize(function () {
            indexView.render();
        });
    });