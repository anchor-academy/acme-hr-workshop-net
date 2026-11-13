/** @type {import('next').NextConfig} */
// NEXT_OUTPUT=export builds a static site served by the API container at /app (Render, no CORS).
const exported = process.env.NEXT_OUTPUT === "export";
export default {
  reactStrictMode: true,
  ...(exported ? { output: "export", trailingSlash: true, basePath: "/app" } : {}),
  env: { NEXT_PUBLIC_BASE_PATH: exported ? "/app" : "" },
};
