/** Vue remote  */
Vue.prototype.$http = axios;

/** axios config */
//axios.defaults.withCredentials = true;
axios.defaults.baseURL = 'http://localhost'

/**
 * 
 * load the English as the default language
 */
//ELEMENT.locale(ELEMENT.lang.en);