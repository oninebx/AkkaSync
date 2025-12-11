const fs = require('fs');
const postcss = require('postcss');
const tailwind = require('@tailwindcss/postcss');

async function run() {
  try {
    // pass config path explicitly to ensure the correct config is loaded
    const plugin = tailwind({ config: './tailwind.config.cjs' });
    const input = '@tailwind base;\n@tailwind components;\n@tailwind utilities;';
    const result = await postcss([plugin]).process(input, { from: undefined });
    fs.writeFileSync('tw-out.css', result.css, 'utf8');
    console.log('tw-out.css written');
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

run();
