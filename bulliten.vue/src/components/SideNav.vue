<template>
  <v-card outlined>
    <v-navigation-drawer
      floating
      permanent
    >
      <v-list>
        <v-list-item
          v-for="item in items"
          :key="item.title"
          link
        >
          <v-list-item-icon>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-icon>

          <v-list-item-content>
            <v-list-item-title @click="navigate(item.path)">
              {{ item.title }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>

      <template v-slot:append>
        <div class="pa-2">
          <v-btn
            color="error"
            outlined
            block
          >
            Logout
          </v-btn>
        </div>
      </template>
    </v-navigation-drawer>
  </v-card>
</template>

<script lang="ts">
import Vue from "vue";
import { mapState } from "vuex";

export default Vue.extend({
  computed: {
    ...mapState("auth", [
      "user"
    ]),
    items() {
      return [
        { title: "Dashboard", icon: "mdi-view-dashboard", path: "/dashboard" },
        { title: "Profile", icon: "mdi-account", path: `/profile/${this.user.username}` },
        { title: "About", icon: "mdi-help-box", path: "/about" }
      ];
    }
  },
  methods: {
    navigate(path: string) {
      this.$router.push(path);
    }
  }
});
</script>
