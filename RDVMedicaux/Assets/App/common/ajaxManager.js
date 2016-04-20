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
define(['jquery', 'common/notify'], function ($, notify) {

    // return module
    var mod = {

        // *** public props ***
        /**
         * Constante des codes erreur transmis par le serveur.
         * GENERICSERVER par défaut
         * @property {Object} ServerErrorCode
         * @readonly
         */
        ServerErrorCode: {
            /**
             * @property {Number} GENERICSERVER
             * @readonly
             * Code par defaut
             */
            GENERICSERVER: 0,
            /**
             * @property {Number} CONCURRENTACCESS
             * @readonly
             * Concurrent access in db
             */
            CONCURRENTACCESS: 1,
            /**
             * @property {Number} UNIQUEKEYCONSTRAINT
             * @readonly
             * Unique key constraint in db
             */
            UNIQUEKEYCONSTRAINT: 2,
            /**
             * @property {Number} DELETEFOREIGNKEY
             * @readonly
             * Foreign key contraint in db
             */
            DELETEFOREIGNKEY: 3,
            /**
             * @property {Number} SESSIONTIMEOUT
             * @readonly
             * Session timeout
             */
            SESSIONTIMEOUT: 4,
            /**
            * @property {Number} UNKNOWNUSER
            * @readonly
            * Authentication user failed
            */
            UNKNOWNUSER: 5,
            /**
             * @property {Number} CUSTOMVALIDATIONFAILED
             * @readonly
             * Echec de la validation custom du message
             */
            CUSTOMVALIDATIONFAILED: 6,
            /**
             * @property {Number} MODELSTATEFAILED
             * @readonly
             * Echec validation model state
             */
            MODELSTATEFAILED: 7,
            /**
             * @property {Number} ACCESSDENIED
             * @readonly
             * Echec dans les droits d'accès
             */
            ACCESSDENIED: 8,
            /**
             * @property {Number} NODATAFOUND
             * @readonly
             * Pas de données trouvées
             */
            NODATAFOUND: 9
        },

        /**
        * @property {Number} ajaxCount
        * Nombre d'appel ajax en cours
        */
        ajaxCount: 0,

        /**
        * @property {Object} $indicator
        * Indicateur de chargement
        */
        $indicator: null,

        // *** public funcs ***
        /**
        * Applique la configuration sur les gestionnaires d'event globaux jquery
        * @method initialize
        */
        initialize: function () {
            var me = this;

            me.$indicator = $('#loading-indicator');

            // gestion indicateur de chargement
            // plusieurs chargement en parallèle possibles
            $(document).ajaxSend(function () { me.showIndicator.call(me); });

            $(document).ajaxComplete(function () { me.hideIndicator.call(me); });

            // gestion des erreurs HTTP
            $(document).ajaxError(function (event, jqxhr, settings, exception) {
                // TODO à completer
                notify.error('une erreur http est survenue...' + exception);
            });

            // Disable caching of AJAX responses
            $.ajaxSetup({
                cache: false
            });

            // HTTP 200 mais gestion des erreurs applicative : success == false
            // function (event, jqxhr, ajaxOptions)
            $(document).ajaxSuccess(function (event, jqxhr) {
                var msg,
                    preventDefault = false;

                // si message de type JSON
                if (/application\/json/.test(jqxhr.getResponseHeader('Content-Type'))) {

                    // parse message pour lecture info success true ou false
                    msg = JSON.parse(jqxhr.responseText);

                    if (msg.success === false) {
                        // une gestion custom customErrorHandler est définie, elle peut stopper la gestion par défaut
                        if (msg.errorObject && jqxhr.customErrorHandler) {
                            preventDefault = jqxhr.customErrorHandler(msg.errorObject, me.ServerErrorCode);
                        }

                        if (!preventDefault) {

                            // modelstate failed.
                            // rien systématiquement. gestion dans traitement spécifique de l'appel ajax
                            if (msg.errorObject.errorCode === me.ServerErrorCode.MODELSTATEFAILED) {
                                return;
                            }

                            // validation custom failed. Affichage d'un message dans une notification
                            if (msg.errorObject.errorCode === me.ServerErrorCode.CUSTOMVALIDATIONFAILED) {
                                notify.validation(msg.errorObject.friendlyMessage);
                            } else {
                                if (msg.errorObject.friendlyMessage !== null) {
                                    notify.error(msg.errorObject.friendlyMessage);
                                }
                            }
                        }
                    }
                }
            });
        },

        /**
        * Masque l'indicateur de progès ajax
        * @method hideIndicator
        */
        hideIndicator: function () {
            this.ajaxCount -= 1;

            // plus aucun appel en cours
            if (this.ajaxCount === 0) {
                this.$indicator.hide();
            }
        },

        /**
        * Affiche l'incidateur de progrès ajax
        * @method showIndicator
        */
        showIndicator: function () {
            if (this.ajaxCount === 0) {
                this.$indicator.show();
            }

            this.ajaxCount += 1;
        },


        /**
        * Execute l'appel ajax avec config POST + JSON
        * et retourne l'objet jQuery XMLHttpRequest (jqXHR)
        * @method postJson
        * @param {String} url L'URL de la requète ajax
        * @param {Object} data Les données de la requête Ajax
        * @param {Array} ignore Optionnel. Propriétés à ignorer lors de l'envoi
        */
        postJson: function (url, data, ignore) {
            if (ignore) {
                data = this.ignore(data, ignore, '_');
            }

            return $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                // correction Bug IE8 JSON.stringify
                // to return "" instead of "null" avec IE8! ===> function (k, v) { return v === "" ? "" : v }
                data: JSON.stringify(data, function (k, v) { return v === "" ? "" : v; })
            });
        },

        /**
        * Execute l'appel ajax avec POST standard
        * et retourne l'objet jQuery XMLHttpRequest (jqXHR)
        * @method post
        * @param {String} url L'URL de la requète ajax
        * @param {Object} data Les données de la requête Ajax
        * @param {Array} ignore Optionnel. Propriétés à ignorer lors de l'envoi
        */
        post: function (url, data, ignore) {

            if (ignore) {
                data = this.ignore(data, ignore, '_');
            }

            return $.ajax({
                url: url,
                type: 'POST',
                data: data
            });
        },

        /**
        * Traitement générique suite à submit de formulaire
        * @method processSubmit
        * @param {Object} msg Objet message vue modèle
        * @param {Object} callBackOk Function à executer en cas de succès.
        * @param {Object} callBackValidationFailed Function à executer en cas d'échec de validation modelstate ou custom.
        */
        processValidate: function (msg, callBackOk, callBackFailed) {
            if (msg.success) {
                callBackOk();
            }
            else if (msg.errorObject && (
                msg.errorObject.errorCode === this.ServerErrorCode.CUSTOMVALIDATIONFAILED ||
                msg.errorObject.errorCode === this.ServerErrorCode.MODELSTATEFAILED)
                ) {
                callBackFailed();
            }
        },


        /**
        * Suppression de propriétés dans l'objet à partir
        * - d'une liste de champ
        * - d'un prefixe
        * @method processSubmit
        * @param {Object} obj Objet à traiter
        * @param {Array} arrayIgnore Propriétés à ignorer ex: ['field1','field2']
        * @param {Array} ignoreIfPrefix Prefixe indiquant les proriétés à ignorer. ex : _
        */
        ignore: function (obj, arrayIgnore, ignoreIfPrefix) {
            var i, index;

            for (i = 0; i < arrayIgnore.length; i++) {
                if (obj[arrayIgnore[i]]) {
                    delete obj[arrayIgnore[i]];
                }
            }

            if (ignoreIfPrefix) {
                for (index in obj) {
                    if (index.indexOf(ignoreIfPrefix) === 0) { // startWith ignoreIfPrefix
                        delete obj[index];
                    }
                }
            }

            return obj;
        }
    };

    // AUTO INITIALIZE
    mod.initialize();

    return mod;
});