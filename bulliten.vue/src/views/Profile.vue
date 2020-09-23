<template>
  <div>
    <v-container fluid>
      <v-alert v-if="errorMsg" type="error">
        {{ errorMsg }}
      </v-alert>

      <v-card outlined>
        <v-card-title>{{ username }}</v-card-title>

        <v-card-actions v-if="!isOwnProfile">
          <v-btn
            text
            @click="() => { profile.isFollowed ? unfollow() : follow() }"
          >
            {{ profile.isFollowed ? "Unfollow" : "Follow" }}
          </v-btn>
        </v-card-actions>

        <v-card-text>Followers: {{ profile.followerCount }} </v-card-text>
        <v-card-text>Following: {{ profile.followingCount }} </v-card-text>
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
    profile: {} as UserProfile,
    posts: [] as Post[],
    errorMsg: ""
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
      const profile = await api.getUserProfile(this.username);
      const posts = await api.getPublicFeed(this.username);

      this.profile = profile;
      this.posts = posts;
    }
    catch (error) {
      this.errorMsg = error.message;
    }
  },
  methods: {
    async follow() {
      try {
        await api.followUser(this.profile.user.username);
        this.profile.isFollowed = true;
        this.profile.followerCount++;
      }
      catch (error) {
        this.errorMsg = error.message;
      }
    },
    async unfollow() {
      try {
        await api.unfollowUser(this.profile.user.username);
        this.profile.isFollowed = false;
        this.profile.followerCount--;
      }
      catch (error) {
        this.errorMsg = error.message;
      }
    }
  }
});
</script>
