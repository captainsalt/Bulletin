<template>
  <v-card
    class="mx-auto"
    max-width="500"
    outlined
    tile
    style="margin: -1px"
  >
    <v-list-item>
      <v-list-item-content>
        <v-list-item-title class="headline mb-1">
          From: {{ post.author.username }}
        </v-list-item-title>
        <v-card-text class="headline font-weight-bold">
          {{ post.content }}
        </v-card-text>
      </v-list-item-content>
    </v-list-item>

    <v-card-actions>
      <v-btn
        text
        @click="isRePosted ? unrepost() : repost()"
      >
        {{ isRePosted ? "Unrepost" : "Repost" }}
      </v-btn>

      <v-btn
        text
        @click="isLiked ? unlike() : like()"
      >
        {{ isLiked ? "Unlike" : "Like" }}
      </v-btn>
    </v-card-actions>

    <v-card-text>
      Reposts: {{ post.rePosts }}
      Likes: {{ post.likes }}
    </v-card-text>

    <v-card-subtitle>
      {{ timestamp }}
    </v-card-subtitle>
  </v-card>
</template>

<script lang="ts">
import Vue, { PropType } from "vue";
import * as api from "@/services/api-interface";

export default Vue.extend({
  props: {
    post: {
      type: Object as PropType<Post>,
      required: true
    }
  },
  computed: {
    isLiked(): boolean {
      return this.post.likeStatus;
    },
    isRePosted(): boolean {
      return this.post.rePostStatus;
    },
    timestamp(): string {
      const date = new Date(this.post.creationDate);
      return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
  },
  methods: {
    like() {
      api.likePost(this.post.id);
      this.post.likeStatus = true;
      this.post.likes++;
    },
    unlike() {
      api.unlikePost(this.post.id);
      this.post.likeStatus = false;
      this.post.likes--;
    },
    repost() {
      api.repost(this.post.id);
      this.post.rePostStatus = true;
      this.post.rePosts++;
    },
    unrepost() {
      api.unRepost(this.post.id);
      this.post.rePostStatus = false;
      this.post.rePosts--;
    }
  }
});
</script>
