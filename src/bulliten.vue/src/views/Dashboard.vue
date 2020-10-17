<template>
  <SideNavLayout>
    <BullitenBoard :posts="posts"/>

    <v-btn
      v-resize="setButtonPos"
      class="create"
      color="secondary"
    >
      Create Post
    </v-btn>
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
  },
  methods: {
    setButtonPos() {
      const createButton = this.$el.querySelector(".create") as HTMLElement;
      const buttonHeight = createButton.offsetHeight;

      createButton.style.top = `calc(100vh - ${buttonHeight}px - 12px)`;
      return buttonHeight;
    }
  }
});
</script>

<style scoped>
.create {
  right: 12px;
  position: fixed;
  z-index: 1;
}
</style>
