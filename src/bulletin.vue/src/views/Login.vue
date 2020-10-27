<template>
  <v-container
    class="d-flex justify-center"
    fill-height
  >
    <LoginForm/>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import LoginForm from "@/components/LoginForm.vue";

export default Vue.extend({
  components: {
    LoginForm
  }
});
</script>
