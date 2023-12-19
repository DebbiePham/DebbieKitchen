
module.exports = {
  content: [
    './Views/**/*.cshtml'
  ],
  theme: {
      extend: {},
  },

  daisyui: {
    themes: ["cupcake", "dark"],
  },

  plugins: [require("daisyui")],
}

