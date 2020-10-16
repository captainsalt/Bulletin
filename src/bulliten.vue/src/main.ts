import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vuetify from "./plugins/vuetify";
import SideNaveLayout from "@/views/Layouts/SideNavLayout.vue";

Vue.config.productionTip = false;

Vue.component("SideNavLayout", SideNaveLayout);

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount("#app");
