/******************************************************
*
* @class common.main.cfgSvc
*
* Module de gestion des informations de configuration
*
*
*******************************************************/
define(["jquery", "common/ajaxManager", "amplify"], function ($, ajaxMgr, amplify) {

    // return module
    return {
        // *** public props ***

        /**
        * @property {Number} nbMaxRow
        * Contient le nombre de ligne maximale renvoyé par une requète
        */
        nbMaxRow: null,

        /**
        * @property {String} urlActionStart
        * Url de l'action du controlleur de chargement de la config
        */
        urlActionStart: APP.baseUrl + 'Api/StartApi/Index',

        /**
        * @property {String} cacheKeyCfg
        * Clé de stockage config en session storage
        */
        cacheKeyCfg: 'cache.cfgrdvmedicaux',

        /**
        * Initialise le module en chargeant les informations depuis le stockage de session local
        * ou depuis le serveur
        * @method initialize
        */
        initialize: function (callback, scope) {
            var me = this,
                cfg = amplify.store.sessionStorage(this.cacheKeyCfg);

            // post json pour obtenir toutes les configs serveur + user
            ajaxMgr.postJson(this.urlActionStart).done(
                function (result) {

                    if (result.success) {
                        cfg = result.data;

                        me.nbMaxRow = cfg.nbMaxRow;
                        me.authCookieName = cfg.authCookieName;

                        // lecture effectuée
                        if (callback) {
                            callback.call(scope);
                        }
                    }
                }
            );
        },

        /**
        * Efface le cache des informations sur l'utilisateur
        * @method clearCache
        */
        clearCache: function () {

        },

        /**
        * Permet d'obtenir la valeur d'un cookie.
        * @method getCookie
        * @param {String} cname Nom du cookie.
        */
        getCookie: function (cname) {
            var name = cname + "=",
                ca = document.cookie.split(';'),
                i, c;

            for (i = 0; i < ca.length; i += 1) {
                c = ca[i].trim();
                if (c.indexOf(name) === 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return "";
        },

        /**
         * Supprime un cookie d'après son nom en appliquant une date d'expiration passée.
         * @param {String} name Nom du cookie à supprimer
         * @param {String} [path] The path pour le cookie.
         * Doit être inclus si un path a été défini pour le cookie.
         */
        clearCookie: function (name, path) {
            if (this.getCookie(name)) {
                path = path || '/';
                document.cookie = name + '=' + '; expires=Thu, 01-Jan-70 00:00:01 GMT; path=' + path;
            }
        },
    };
});