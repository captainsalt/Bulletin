<template>
  <v-card class="card" outlined>
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

      <v-btn
        color="primary"
        block
        @click="submit"
      >
        Submit
      </v-btn>
    </v-form>
  </v-card>
</template>

<script>
import Vue from "vue";
import * as api from "@/services/api-interface";
import { mapMutations } from "vuex";
import { SET_TOKEN } from "@/store/mutations";

export default Vue.extend({
  data: () => ({
    username: "",
    password: "",
    errorMsg: ""
  }),
  methods: {
    ...mapMutations("auth", [
      SET_TOKEN
    ]),
    async submit() {
      const response = await api.createAccount(this.username, this.password);

      if (!response.ok) {
        this.errorMsg = (await response.json()).message;
        return;
      }

      const token = (await response.json()).token;
      this.SET_TOKEN(token);
      await this.$router.push("/dashboard");
    }
  }
});
</script>

<style scoped>
.card {
  width: 50% ;
  padding: 10px;
}
</style>
