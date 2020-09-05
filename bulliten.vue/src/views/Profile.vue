<template>
  <v-container>
    <div>
      <v-alert v-if="errorMsg" type="error">
        {{ errorMsg }}
      </v-alert>

      <v-row>
        <p>
          {{ profileUser.username }}
        </p>
      </v-row>

      <v-row>
        <v-btn @click="follow">
          Follow
        </v-btn>
      </v-row>
    </div>
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
    profileUser: {} as UserAccount,
    errorMsg: ""
  }),
  async beforeMount() {
    try {
      const user = await api.getUser(this.username);
      this.profileUser = user;
    }
    catch (error) {
      this.errorMsg = error.message;
    }
  },
  methods: {
    async follow() {
      try {
        await api.followUser(this.profileUser.username);
      }
      catch (error) {
        this.errorMsg = error.message;
      }
    }
  }
});
</script>
