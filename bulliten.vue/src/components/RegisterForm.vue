<template>
  <v-form>
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

export default Vue.extend({
  data: () => ({
    username: "",
    password: ""
  }),
  methods: {
    submit() {

    }
  }
});
</script>
