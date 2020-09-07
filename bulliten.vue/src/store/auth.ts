/* eslint-disable @typescript-eslint/no-explicit-any */
import { SET_TOKEN, SET_USER } from "./mutations";
import { ActionContext } from "vuex";
import * as api from "@/services/api-interface";

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
  },
  actions: {
    storeAuth({ commit }: ActionContext<any, any>, { token, user }: AuthResponse) {
      commit(SET_TOKEN, token);
      commit(SET_USER, user);
    },
    async login({ dispatch }: ActionContext<any, any>, { username, password }: { username: string; password: string }) {
      try {
        const { token, user } = await api.login(username, password);
        await dispatch("storeAuth", { token, user });
      }
      catch (error) {
        throw error;
      }
    },
    async register({ dispatch }: ActionContext<any, any>, { username, password }: { username: string; password: string }) {
      try {
        const { token, user } = await api.createAccount(username, password);
        await dispatch("storeAuth", { token, user });
      }
      catch (error) {
        throw error;
      }
    }
  }
};
