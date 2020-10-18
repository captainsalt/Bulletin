<template>
  <v-container fill-height fluid>
    <v-row no-gutters>
      <v-col cols="auto">
        <div class="flex sticky">
          <SideNav class="sticky"/>
        </div>
      </v-col>

      <v-col>
        <slot/>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import Vue from "vue";
import SideNav from "@/components/SideNav.vue";

export default Vue.extend({
  components: {
    SideNav
  }
});
</script>

<style scoped>
.flex {
  display: flex;
  align-content: center;
  justify-content: center;
  height: calc(100vh - 24px);
}

.sticky {
  position: sticky;
  top: 12px;
}
</style>


