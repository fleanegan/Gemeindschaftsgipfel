<template>
  <div class="box_around_everything_but_still_inside_app">
    <header class="nav_header">
      <nav class="nav-links">
        <RouterLink class="home-link" to="/"><img alt="Home" src="/public/icon.svg" style="width: 5rem; height: 5rem;">
        </RouterLink>
        <RouterLink to="/about">About</RouterLink>
        <RouterLink to="/login">Login</RouterLink>
        <RouterLink to="/topic">Vortragsthemen</RouterLink>
      </nav>
    </header>
    <div class="routed-elements">
      <RouterView/>
    </div>
  </div>
</template>

<style scoped>
.nav-links {
  display: flex;
  justify-content: flex-start;
  flex-direction: row;
  align-items: center;
  width: 100%;
  padding: 0.5rem;
  padding-right: 1rem;
}

nav a.router-link-exact-active {
  color: var(--color-text);
}

nav a.router-link-exact-active:hover {
  background-color: transparent;
}

.home-link {
  margin-right: auto; /* Pushes other links to the right */
}

.routed-elements {
  padding-left: 0.5rem;
}

.box_around_everything_but_still_inside_app {
  width: 512px; /* Set the default width to 60% */
  max-width: 900px; /* Restrict to 900px if smaller */
  /*background-color: aliceblue;*/
  height: 100vh;
  display: flex;
  flex-direction: column;
}

@media (max-width: 900px) {
  .box_around_everything_but_still_inside_app {
    width: 100%;
  }
}

</style>
