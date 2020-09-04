<template>
  <v-container>
    <v-row>
      {{ profileUser.username }}
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import * as api from "@/services/api-interface";

export default Vue.extend({
  props: {
    username: {
      required: true,
      type: String
    }
  },
  data: () => ({
    profileUser: {} as UserAccount
  }),
  async beforeMount() {
    const user = await api.getUser(this.username);
    this.profileUser = user;
  }
});
</script>
