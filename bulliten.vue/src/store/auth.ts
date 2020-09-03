/* eslint-disable @typescript-eslint/no-explicit-any */
import { SET_TOKEN } from "./mutations";

export default {
  namespaced: true,
  state: {
    token: ""
  },
  mutations: {
    [SET_TOKEN](state: any, payload: string) {
      state.token = payload;
    }
  }
};
