import Vue from "vue";
import Vuex from "vuex";
import VuexPersist from "vuex-persist";
import auth from "./auth";

Vue.use(Vuex);

const persist = new VuexPersist({
  storage: window.localStorage,
  modules: ["auth"]
});

export default new Vuex.Store({
  state: {
  },
  mutations: {
  },
  actions: {
  },
  modules: {
    auth
  },
  plugins: [persist.plugin]
});
