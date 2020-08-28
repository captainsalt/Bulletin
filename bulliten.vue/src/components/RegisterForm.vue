<template>
  <v-form>
    <v-alert v-if="errorMsg" type="error">
      {{ errorMsg }}
    </v-alert>

    <v-text-field
      v-model="username"
      label="Username"
      required
    />

    <v-text-field
      v-model="password"
      type="password"
      label="Password"
      required
    />

    <v-btn @click="submit">
      Submit
    </v-btn>
  </v-form>
</template>

<script>
import Vue from "vue";
import * as api from "@/services/api-interface";
import * as mutations from "@/store/mutations";
import { mapMutations } from "vuex";

export default Vue.extend({
  data: () => ({
    username: "",
    password: "",
    errorMsg: ""
  }),
  methods: {
    ...mapMutations("auth", [
      mutations.SET_TOKEN
    ]),
    async submit() {
      const response = await api.createAccount(this.username, this.password);

      if (!response.ok) {
        const err = await response.json();
        this.errorMsg = err.message;
        return;
      }

      this.SET_TOKEN(await response.text());
    }
  }
});
</script>
