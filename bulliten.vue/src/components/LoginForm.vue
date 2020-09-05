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
        block
        color="primary"
        @click="submit"
      >
        Login
      </v-btn>
    </v-form>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import * as api from "@/services/api-interface";
import { mapActions } from "vuex";

export default Vue.extend({
  data: () => ({
    username: "",
    password: "",
    errorMsg: ""
  }),
  methods: {
    ...mapActions("auth", [
      "storeAuth"
    ]),
    async submit() {
      try {
        const { token, user } = await api.login(this.username, this.password);
        this.storeAuth({ token, user });
        await this.$router.push("/dashboard");
      }
      catch (error) {
        this.errorMsg = error.message;
      }
    }
  }
});
</script>

<style scoped>
.card {
  width: 50%;
  padding: 10px;
}
</style>
