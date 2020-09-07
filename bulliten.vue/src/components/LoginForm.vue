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
import { mapActions } from "vuex";

export default Vue.extend({
  data: () => ({
    username: "",
    password: "",
    errorMsg: ""
  }),
  methods: {
    ...mapActions("auth", [
      "login"
    ]),
    async submit() {
      try {
        await this.login({ username: this.username, password: this.password });
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
