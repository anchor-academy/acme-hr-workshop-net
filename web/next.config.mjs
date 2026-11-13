/** @type {import('next').NextConfig} */
// NEXT_OUTPUT=export builds a static site (Render static site, no cold start).
export default { reactStrictMode: true, ...(process.env.NEXT_OUTPUT === "export" ? { output: "export", trailingSlash: true, basePath: "/app" } : {}) };
