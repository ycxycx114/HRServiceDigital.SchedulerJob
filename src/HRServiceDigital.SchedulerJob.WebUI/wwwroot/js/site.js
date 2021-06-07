// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

/** Vue remote  */
Vue.prototype.$http = axios;

/** axios config */
//axios.defaults.withCredentials = true;
axios.defaults.baseURL = 'http://localhost:5000'

/**
 *
 * load the English as the default language
 */
//ELEMENT.locale(ELEMENT.lang.en);