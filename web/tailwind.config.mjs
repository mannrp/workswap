/** @type {import('tailwindcss').Config} */
const config = {
    darkMode: 'class',
    content: [
        "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
        "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
        "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
    ],
    theme: {
        extend: {
            colors: {
                background: "var(--background)",
                foreground: "var(--foreground)",
                surface: "var(--surface)",
                border: "var(--border)",
                primary: {
                    DEFAULT: "var(--primary)",
                    foreground: "var(--primary-foreground)",
                },
                secondary: {
                    DEFAULT: "var(--secondary)",
                    foreground: "var(--secondary-foreground)",
                },
                muted: {
                    DEFAULT: "var(--muted)",
                    foreground: "var(--muted-foreground)",
                },
                destructive: {
                    DEFAULT: "var(--destructive)",
                    foreground: "var(--destructive-foreground)",
                },
            },
            borderRadius: {
                DEFAULT: "0px",
                none: "0px",
                sm: "0px",
                md: "0px",
                lg: "0px",
                xl: "0px",
                "2xl": "0px",
                "3xl": "0px",
                full: "0px",
            },
            boxShadow: {
                hard: "4px 4px 0px 0px var(--border)",
            }
        },
    },
    plugins: [],
};
export default config;
