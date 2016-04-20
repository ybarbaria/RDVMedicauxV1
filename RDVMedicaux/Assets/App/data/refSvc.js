/******************************************************
*
* @class common.data.refSvc
*
*
*******************************************************/
define(['jquery', 'amplify', 'common/ajaxManager', 'data/mainSvc'], function ($, amplify, ajaxMgr, mainSvc) {
    "use strict";
    var _module,
        _etatCacheKey = 'cache.etat';

    _module = {

        ref: {
            //state: { key: 'cache.villes', url: mainSvc.reference.api.getVilles },
        },

        /**
         * Action chargement d'une combo à partir des données de références
         * @method loadCombo
         * @param {Object} ref Objet de type {key:..., url: ...}.
         * @param {Object} arrayKo Tableau à charger (observableArray pour ko).
         * @param {Function} [filter] Fonction optionnelle pour filtrer les éléments.
         * Cette fonction executée pour chaque élément doit retourner vrai si l'on veut conserver l'élément
         * La fonction reçoit en paramètre l'élément courant
         */
        loadCombo: function (ref, arrayKo, filter) {
            var me = this;

            _module.getDatasCombo(ref, function (datas) {
                arrayKo.removeAll();//purge existant si besoin

                $.each(datas, function (key, val) {
                    // filtre vi un prédicat si il existe
                    if (filter && !filter.call(me, val)) {
                        return true; //next
                    }

                    arrayKo.push(val);
                });
            });
        },

        /**
         * Action chargement d'une combo à partir des données de références avec une fonction de callback
         * @method loadComboCallback
         * @param {Object} ref Objet de type {key:..., url: ...}.
         * @param {Object} arrayKo Tableau à charger (observableArray pour ko).
         * @param {Function} [filter] Fonction optionnelle pour filtrer les éléments.
         * Cette fonction executée pour chaque élément doit retourner vrai si l'on veut conserver l'élément
         * La fonction reçoit en paramètre l'élément courant
         */
        loadComboCallback: function (ref, arrayKo, filter, callback) {
            var me = this;

            _module.getDatasCombo(ref, function (datas) {
                arrayKo.removeAll();//purge existant si besoin

                $.each(datas, function (key, val) {
                    // filtre vi un prédicat si il existe
                    if (filter && !filter.call(me, val)) {
                        return true; //next
                    }

                    arrayKo.push(val);
                });

                callback();
            });
        },

        /**
         * Chargement des données pour une combo depuis le session storage ou depuis le serveur
         * Les données dans les combo sont de types { id: ,label: ,extra: }
         * @method getDatasCombo
         * @param {Object} ref objet de type {key:..., url: ...}.
         * @param {Function} callback Callback de succès de chargement des données.
         */
        getDatasCombo: function (ref, callback) {
            var me = this,
                // données déjà présent dans sessionStorage ?
                datas = amplify.store.sessionStorage(ref.key);

            // chargement ajax si pas en cache
            if (!datas) {
                ajaxMgr.postJson(ref.url).done(function (msg) {
                    if (msg.success) {
                        datas = msg.data;
                        amplify.store.sessionStorage(ref.key, datas);

                        // chargement effectué
                        callback.call(me, datas);
                    }
                });
            } else {
                // chargement effectué
                callback.call(me, datas);
            }
        },

        /**
        * Vidage des données d'une combo 
        * @method clearDatasCombo
        * @param {Object} ref objet de type {key:..., url: ...}.
        */
        clearDatasCombo: function (ref) {
            // On vide le cache de la combo
            amplify.store.sessionStorage(ref.key, null);
        }


    };

    return _module;
});