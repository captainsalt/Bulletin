<template>
  <div>
    <v-container fluid>
      <v-alert v-if="errorMsg" type="error">
        {{ errorMsg }}
      </v-alert>

      <v-card outlined>
        <v-card-title>{{ profileUser.username }}</v-card-title>

        <v-card-actions>
          <v-btn text @click="follow">
            Follow
          </v-btn>
        </v-card-actions>

        <v-card-text>Followers: {{ followers }} </v-card-text>
        <v-card-text>Following: {{ following }} </v-card-text>
      </v-card>
    </v-container>
  </div>
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
    errorMsg: "",
    followers: 0,
    following: 0
  }),
  async beforeMount() {
    try {
      const user = await api.getUser(this.username);
      const followInfo = await api.getFollowInfo(this.username);

      this.followers = followInfo.followers;
      this.following = followInfo.following;
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
