/***************************************************
 *
 * @class data.mainSvc
 *
 * Module gestion des routes d'accès aux services
 *
 *
 ****************************************************/
define(function () {

    var rootApi = APP.baseUrl,
        rootTemplate = APP.baseUrl;

    return {
        activity: {
            api: {
                detail: rootApi + 'Api/ActivityApi/Detail',
                filter: rootApi + 'Api/ActivityApi/Filter',
                start: rootApi + 'Api/ActivityApi/Start',
                end: rootApi + 'Api/ActivityApi/End',
                yes: rootApi + 'Api/ActivityApi/Yes',
                no: rootApi + 'Api/ActivityApi/No',
                toend: rootApi + 'Api/ActivityApi/ToEnd'
            },
            template: {
                index: rootTemplate + 'Activity/Index',
                detail: rootTemplate + 'Activity/Detail',
                newreserve: rootTemplate + 'Reserve/Index'
            }
        }
    };
});