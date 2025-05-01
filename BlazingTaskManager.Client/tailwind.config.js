/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./**/*.{razor, html}'],
  theme: {
      extend: {
          colors: {
              'bt-background': '#3f3d47',
              'bt-black-50': '#858586',
              'bt-black-100': '#69696a',
              'bt-black-200': '#4f4f50',
              'bt-black-300': '#363637',
              'bt-black-400': '#1e1e1f',
              'bt-black-400': '#010103',
              'bt-purple-50': '#c7bbf9',
              'bt-purple-100': '#b8aaf7',
              'bt-purple-200': '#a99af4',
              'bt-purple-300': '#998af2',
              'bt-purple-400': '#887af0',
              'bt-purple-500': '#766bed',
          },
      },
      fontFamily: {
          sans: ["'Libre Franklin'", "sans-serif"]
      }
  },
  plugins: [],
}

