<template>
  <div class="navbar-container">
    <b-navbar fixed="top" toggleable="lg" :class="{scrolled: scrollPosition > 80}">
      <b-navbar-brand to="stats"></b-navbar-brand>
      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav class="ml-auto">
          <b-nav-item to="stats">Domov</b-nav-item>
          <b-nav-item to="map">Zemljevid</b-nav-item>
          <b-nav-item to="tables">Tabela</b-nav-item>
          <b-nav-item to="about">O projektu</b-nav-item>
          <b-nav-item to="sources">Viri</b-nav-item>
          <b-nav-item to="links">Povezave</b-nav-item>
          <b-nav-item to="team">Ekipa</b-nav-item>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
  </div>
</template>

<script>
export default {
  name: "Navbar",
  props: {
    msg: String
  },
  data() {
    return {
      scrollPosition: ""
    };
  },
  created() {
    window.addEventListener("scroll", this.handleScroll);
  },
  methods: {
    handleScroll() {
      this.scrollPosition = window.scrollY;
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss">
.navbar-container {
  overflow: hidden;
}
nav.navbar {
  background: $yellow;
  transition: all 0.35s ease-out;

  &.scrolled {
    padding-top: 0;
    padding-bottom: 0;
  }

  &.show {
    background: $yellow !important;
  }

  .navbar-expand-lg {
    background: $yellow !important;
  }

  .navbar-brand {
    background: url("../assets/svg/covid-19.svg") no-repeat;
    width: 145px;
    height: 40px;
    margin: 10px 0;
  }

  a.nav-link {
    position: relative;
    display: inline-block;
    color: rgba(0, 0, 0, 0.5);
    line-height: 20px;
    font-weight: 600;

    &:hover {
      color: #000000;
    }

    &:focus {
      outline: none;
    }

    &.router-link-active {
      color: #000000;
      
      &:after {
        content: "";
        position: absolute;
        display: block;
        left: 0;
        right: 0;
        border-bottom: 10px solid #FFFFFF;

        @media (min-width: 992px) {
          left: 0.5rem;
          right: 0.5rem;
        }
      }
    }
  }
}
</style>
