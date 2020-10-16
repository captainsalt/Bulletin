<template>
  <SideNavLayout>
    <BullitenBoard :posts="posts"/>
  </SideNavLayout>
</template>

<script lang="ts">
import Vue from "vue";
import BullitenBoard from "@/components/BullitenBoard.vue";
import * as api from "@/services/api-interface";
import { mapState } from "vuex";

export default Vue.extend({
  components: {
    BullitenBoard
  },
  data: () => ({
    posts: [] as Post[]
  }),
  computed: {
    ...mapState("auth", [
      "user"
    ])
  },
  async beforeMount() {
    this.posts = await api.getPersonalFeed();
  }
});
</script>

