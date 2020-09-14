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
      <v-btn text>
        Re-Post
      </v-btn>
      <v-btn text @click="like">
        Like
      </v-btn>
    </v-card-actions>

    <v-card-text>
      Likes: {{ post.likes }}
    </v-card-text>

    <v-card-subtitle>
      {{ readableTime(post.creationDate) }}
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
  methods: {
    readableTime(dateString: string): string {
      const date = new Date(dateString);
      return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    },
    like() {
      api.likePost(this.post.id);
    }
  }
});
</script>
