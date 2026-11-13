"use client";
import { useEffect, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import { currentEmployee, Employee, signOut } from "@/lib/api";

/** Sends signed-out visitors to /login and shows who is signed in. */
export function AuthGate({ children }: { children: React.ReactNode }) {
  const path = usePathname();
  const router = useRouter();
  const onLogin = path.replace(/\/$/, "").endsWith("/login");
  const [me, setMe] = useState<Employee | null>(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    const who = currentEmployee();
    setMe(who);
    setReady(true);
    if (!who && !onLogin) router.replace("/login");
  }, [path, onLogin, router]);

  if (!ready) return null;
  if (!me && !onLogin) return null;
  return (
    <>
      {me && !onLogin && (
        <div className="whoami">
          Signed in as <b>{me.name}</b> · {(me.roles ?? []).join(", ")} · <button className="link" onClick={signOut}>Sign out</button>
        </div>
      )}
      {children}
    </>
  );
}
