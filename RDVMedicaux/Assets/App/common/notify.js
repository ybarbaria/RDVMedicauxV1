/******************************************************
*
* @class common.main.ajaxManager
*
* Module de gestion des appels ajax via jquery
* - affichage d'un temoin de chargement
* - gestion global des erreurs
*
* @uses jquery
*
*******************************************************/
define(['jquery'], function ($) {

    var _module;

    /**
    *   Fonction permetant de surelever les messages les uns au dessus des autres.
    *   @param id : identifant du message
    *   @param hauteur : hauteur de positionnement du premier message
    */
    function move(id, hauteur) {
        if ($("#" + id).length > 0) {
            return move(id + "_1", hauteur + $("#" + id).outerHeight() + 5);
        }
        return { _id: id, _hauteur: hauteur };
    }

    /**
    *   Fonction affichant un message.
    *   @param message : contenu html du message.
    *   @param timeOut : duree de persistance du message, infini si non int
    *   @param type : le type du message parmis : {info, danger, warning, success}
    */
    function notify(message, timeOut, type) {
        var obj = move('myAlert', 5);
        $("body").append('<div id="' + obj._id + '" class="alert alert-' + type +
            ' fade in" style="position: fixed; right: 15px; bottom: ' + obj._hauteur +
            'px; width: 350px; z-index: 2000; padding: 10px 10px; min-height: 50px;">   <a href="#" class="close" data-dismiss="alert">&times;</a>   ' +
            message + '</div>');
        if ($.isNumeric(timeOut)) {
            setTimeout(function () { $('#' + obj._id).alert('close'); }, timeOut);
        }
    }

    _module = {
        /**
        *   Fonction affichant un message de type : success
        *   @param message : contenu html du message.
        *   @param timeOut : duree de persistance du message, infini si non int
        */
        success: function (message, timeOut) {
            timeOut = timeOut || 3000;
            notify(message, timeOut, "success");
        },

        /**
        *   Fonction affichant un message de type : error
        *   @param message : contenu html du message.
        *   @param timeOut : duree de persistance du message, infini si non int
        */
        error: function (message) {
            notify(message, "", "danger");
        },

        /**
        *   Fonction affichant un message de type : info
        *   @param message : contenu html du message.
        *   @param timeOut : duree de persistance du message, infini si non int
        */
        info: function (message, timeOut) {
            timeOut = timeOut || 5000;
            notify(message, timeOut, "info");
        },

        /**
        *   Fonction affichant un message de type : warning
        *   @param message : contenu html du message.
        *   @param timeOut : duree de persistance du message, infini si non int
        */
        warning: function (message, timeOut) {
            timeOut = timeOut || 10000;
            notify(message, timeOut, "warning");
        }
    };

    return _module;
});