<template>
  <div>
    <v-container fluid>
      <v-alert v-if="errorMsg" type="error">
        {{ errorMsg }}
      </v-alert>

      <v-card outlined>
        <v-card-title>{{ profileUser.username }}</v-card-title>

        <v-card-actions v-if="!isOwnProfile">
          <v-btn text @click="follow">
            Follow
          </v-btn>
        </v-card-actions>

        <v-card-text>Followers: {{ followers.length }} </v-card-text>
        <v-card-text>Following: {{ following.length }} </v-card-text>
      </v-card>
    </v-container>

    <v-container>
      <BullitenBoard :posts="posts"/>
    </v-container>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import * as api from "@/services/api-interface";
import { mapState } from "vuex";
import BullitenBoard from "@/components/BullitenBoard.vue";

export default Vue.extend({
  components: {
    BullitenBoard
  },
  props: {
    username: {
      required: true,
      type: String
    }
  },
  data: () => ({
    profileUser: {} as UserAccount,
    posts: [] as Array<Post>,
    errorMsg: "",
    followers: [] as Array<UserAccount>,
    following: [] as Array<UserAccount>
  }),
  computed: {
    ...mapState("auth", {
      authUser: "user"
    }),
    isOwnProfile() {
      return this.authUser.username === this.username;
    }
  },
  async beforeMount() {
    try {
      const user = await api.getUser(this.username);
      const followInfo = await api.getFollowInfo(this.username);
      const posts = await api.getPublicFeed(this.username);

      this.followers = followInfo.followers;
      this.following = followInfo.following;
      this.profileUser = user;
      this.posts = posts;
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
