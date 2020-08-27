<template>
  <v-container
    class="d-flex justify-center"
    fill-height
  >
    <RegisterForm/>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import RegisterForm from "@/components/RegisterForm.vue";

export default Vue.extend({
  components: {
    RegisterForm
  },
  data: () => ({

  })
});
</script>

<style>
</style>
