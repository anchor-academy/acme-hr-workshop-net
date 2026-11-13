import "./globals.css";
import Link from "next/link";
import { AuthGate } from "@/components/AuthGate";

export const metadata = { title: "Acme HR" };

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <nav>
          <span className="brand">Acme HR</span>
          <Link href="/">Home</Link>
          <Link href="/employees">Employees</Link>
          <Link href="/leave">Leave</Link>
          {/* Labs add: Helpdesk (Lab 2), Policies (Lab 3), Calendar (Lab 4), Documents and Review (Lab 6) */}
        </nav>
        <main><AuthGate>{children}</AuthGate></main>
      </body>
    </html>
  );
}
