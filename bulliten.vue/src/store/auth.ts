/* eslint-disable @typescript-eslint/no-explicit-any */
import { SET_TOKEN, SET_USER } from "./mutations";

export default {
  namespaced: true,
  state: {
    token: "",
    user: {} as UserAccount
  },
  mutations: {
    [SET_TOKEN](state: any, payload: string) {
      state.token = payload;
    },
    [SET_USER](state: any, payload: UserAccount) {
      state.user = payload;
    }
  }
};
