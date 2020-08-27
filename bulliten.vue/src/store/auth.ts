import { SET_TOKEN } from "./mutations";

export default {
  namespaced: true,
  state: {
    token: localStorage.getItem("token")
  },
  mutations: {
    [SET_TOKEN](state: any, payload: string) {
      state.token = payload;
    }
  }
}
