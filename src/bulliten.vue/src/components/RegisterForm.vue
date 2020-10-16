<template>
  <FormShell>
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
      Register
    </v-btn>
  </FormShell>
</template>

<script>
import Vue from "vue";
import FormShell from "@/components/FormShell.vue";
import { mapActions } from "vuex";

export default Vue.extend({
  components: {
    FormShell
  },
  data: () => ({
    username: "",
    password: "",
    errorMsg: ""
  }),
  methods: {
    ...mapActions("auth", [
      "register"
    ]),
    async submit() {
      try {
        await this.register({ username: this.username, password: this.password });
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
