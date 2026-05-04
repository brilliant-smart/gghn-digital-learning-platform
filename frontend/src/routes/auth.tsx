import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert";
import { CircleAlert, Lock, Mail, Clock, ShieldAlert } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { ApiError } from "@/api/client";

type AuthError = {
  category: "credentials" | "unverified" | "locked" | "rate_limited" | "validation" | "general";
  title: string;
  message: string;
  action?: string;
  details?: string[];
  lockoutEnd?: string;
  email?: string;
};

function classifyError(err: unknown): AuthError {
  if (err instanceof ApiError) {
    switch (err.status) {
      case 401:
        return {
          category: "credentials",
          title: "Sign in failed",
          message: err.detail || "The email or password you entered is incorrect. Please try again.",
        };
      case 403:
        return {
          category: "unverified",
          title: "Email not verified",
          message: err.detail || "Please verify your email address before signing in.",
          action: "Resend verification email",
          email: err.email,
        };
      case 423:
        return {
          category: "locked",
          title: "Account locked",
          message: err.detail || "Your account has been locked due to too many failed login attempts.",
          lockoutEnd: err.lockoutEnd,
        };
      case 429:
        return {
          category: "rate_limited",
          title: "Too many attempts",
          message: "You've made too many requests. Please wait a moment and try again.",
        };
      case 422:
        return {
          category: "validation",
          title: err.title || "Registration failed",
          message: err.detail || "Please fix the following issues and try again.",
          details: err.errors,
        };
      default:
        return {
          category: "general",
          title: err.title || "Something went wrong",
          message: err.detail || "An unexpected error occurred. Please try again.",
        };
    }
  }

  if (err instanceof Error) {
    return {
      category: "general",
      title: "Something went wrong",
      message: err.message || "An unexpected error occurred. Please try again.",
    };
  }

  return {
    category: "general",
    title: "Something went wrong",
    message: "An unexpected error occurred. Please try again.",
  };
}

export const Route = createFileRoute("/auth")({
  head: () => ({
    meta: [
      { title: "Sign In | GGHN Digital Learning" },
      { name: "description", content: "Sign in or create an account on the GGHN Digital Learning Platform." },
    ],
  }),
  component: AuthPage,
});

function LockoutCountdown({ lockoutEnd }: { lockoutEnd?: string }) {
  const [remaining, setRemaining] = useState<number | null>(null);

  if (!lockoutEnd) return null;

  const endTime = new Date(lockoutEnd).getTime();

  if (remaining === null) {
    const diff = Math.max(0, Math.ceil((endTime - Date.now()) / 1000));
    setRemaining(diff);
  }

  if (remaining !== null && remaining > 0) {
    setTimeout(() => setRemaining(Math.max(0, Math.ceil((endTime - Date.now()) / 1000))), 1000);
  }

  if (remaining === 0) return <span className="text-green-600 font-medium">You can try signing in again now.</span>;

  const minutes = Math.floor(remaining! / 60);
  const seconds = remaining! % 60;

  return (
    <span className="font-medium">
      Try again in {minutes > 0 ? `${minutes}m ${seconds}s` : `${seconds}s`}.
    </span>
  );
}

function AuthPage() {
  const { isAuthenticated, login, register } = useAuth();
  const navigate = useNavigate();
  const [mode, setMode] = useState<"login" | "register">("login");
  const [authError, setAuthError] = useState<AuthError | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [resending, setResending] = useState(false);
  const [resent, setResent] = useState(false);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");
  const [organization, setOrganization] = useState("");
  const [jobTitle, setJobTitle] = useState("");
  const [country, setCountry] = useState("");

  if (isAuthenticated) {
    navigate({ to: "/dashboard", replace: true });
    return null;
  }

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setAuthError(null);
    setSubmitting(true);
    try {
      await login(email, password);
      navigate({ to: "/dashboard", replace: true });
    } catch (err: unknown) {
      setAuthError(classifyError(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setAuthError(null);
    setSubmitting(true);
    try {
      await register({
        email: regEmail,
        password: regPassword,
        firstName,
        lastName,
        organization: organization || undefined,
        jobTitle: jobTitle || undefined,
        country: country || undefined,
      });
      navigate({ to: "/dashboard", replace: true });
    } catch (err: unknown) {
      setAuthError(classifyError(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleResendVerification = async () => {
    if (!authError?.email) return;
    setResending(true);
    try {
      const { api } = await import("@/api/client");
      await api.post("/auth/resend-verification", { email: authError.email });
      setResent(true);
    } catch {
      setResent(true);
    } finally {
      setResending(false);
    }
  };

  const renderError = () => {
    if (!authError) return null;

    const iconMap = {
      credentials: <CircleAlert className="h-4 w-4" />,
      unverified: <Mail className="h-4 w-4" />,
      locked: <Lock className="h-4 w-4" />,
      rate_limited: <Clock className="h-4 w-4" />,
      validation: <ShieldAlert className="h-4 w-4" />,
      general: <CircleAlert className="h-4 w-4" />,
    };

    const colorMap: Record<AuthError["category"], string> = {
      credentials: "border-destructive/50 text-destructive [&>svg]:text-destructive",
      unverified: "border-blue-500/50 text-blue-700 dark:text-blue-400 [&>svg]:text-blue-500 bg-blue-50 dark:bg-blue-950/30",
      locked: "border-amber-500/50 text-amber-700 dark:text-amber-400 [&>svg]:text-amber-500 bg-amber-50 dark:bg-amber-950/30",
      rate_limited: "border-orange-500/50 text-orange-700 dark:text-orange-400 [&>svg]:text-orange-500 bg-orange-50 dark:bg-orange-950/30",
      validation: "border-destructive/50 text-destructive [&>svg]:text-destructive",
      general: "border-destructive/50 text-destructive [&>svg]:text-destructive",
    };

    const variant = authError.category === "credentials" || authError.category === "validation" || authError.category === "general"
      ? "destructive" as const
      : "default" as const;

    return (
      <Alert variant={variant} className={`mb-4 ${colorMap[authError.category]}`}>
        {iconMap[authError.category]}
        <AlertTitle>{authError.title}</AlertTitle>
        <AlertDescription>
          <p>{authError.message}</p>
          {authError.category === "locked" && <LockoutCountdown lockoutEnd={authError.lockoutEnd} />}
          {authError.category === "unverified" && authError.email && (
            <div className="mt-2">
              {resent ? (
                <p className="text-sm font-medium text-green-600 dark:text-green-400">
                  If an account exists for {authError.email}, a verification email has been sent.
                </p>
              ) : (
                <Button
                  variant="outline"
                  size="sm"
                  className="mt-1 border-blue-300 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:text-blue-400 dark:hover:bg-blue-950"
                  onClick={handleResendVerification}
                  disabled={resending}
                >
                  {resending ? "Sending..." : "Resend verification email"}
                </Button>
              )}
            </div>
          )}
          {authError.details && authError.details.length > 0 && (
            <ul className="mt-2 list-disc pl-4 space-y-0.5">
              {authError.details.map((d, i) => (
                <li key={i}>{d}</li>
              ))}
            </ul>
          )}
        </AlertDescription>
      </Alert>
    );
  };

  return (
    <SiteShell>
      <div className="flex min-h-[60vh] items-center justify-center px-4 py-12">
        <Card className="w-full max-w-md">
          <CardHeader className="text-center">
            <CardTitle className="text-2xl">
              {mode === "login" ? "Welcome back" : "Create your account"}
            </CardTitle>
            <CardDescription>
              {mode === "login"
                ? "Sign in to track your learning progress"
                : "Join the GGHN Digital Learning Platform"}
            </CardDescription>
          </CardHeader>
          <CardContent>
            {renderError()}

            {mode === "login" ? (
              <form onSubmit={handleLogin} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="email">Email</Label>
                  <Input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required placeholder="you@example.com" />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="password">Password</Label>
                  <Input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required placeholder="Your password" />
                </div>
                <Button type="submit" className="w-full" disabled={submitting}>
                  {submitting ? "Signing in..." : "Sign in"}
                </Button>
                <p className="text-center text-sm text-muted-foreground">
                  Don't have an account?{" "}
                  <button type="button" className="text-primary hover:underline" onClick={() => { setMode("register"); setAuthError(null); }}>
                    Register
                  </button>
                </p>
              </form>
            ) : (
              <form onSubmit={handleRegister} className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-2">
                    <Label htmlFor="firstName">First name</Label>
                    <Input id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="lastName">Last name</Label>
                    <Input id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
                  </div>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="regEmail">Email</Label>
                  <Input id="regEmail" type="email" value={regEmail} onChange={(e) => setRegEmail(e.target.value)} required placeholder="you@example.com" />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="regPassword">Password</Label>
                  <Input id="regPassword" type="password" value={regPassword} onChange={(e) => setRegPassword(e.target.value)} required minLength={8} placeholder="Min 8 characters" />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="organization">Organization <span className="text-muted-foreground">(optional)</span></Label>
                  <Input id="organization" value={organization} onChange={(e) => setOrganization(e.target.value)} />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-2">
                    <Label htmlFor="jobTitle">Job title <span className="text-muted-foreground">(optional)</span></Label>
                    <Input id="jobTitle" value={jobTitle} onChange={(e) => setJobTitle(e.target.value)} />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="country">Country <span className="text-muted-foreground">(optional)</span></Label>
                    <Input id="country" value={country} onChange={(e) => setCountry(e.target.value)} />
                  </div>
                </div>
                <Button type="submit" className="w-full" disabled={submitting}>
                  {submitting ? "Creating account..." : "Create account"}
                </Button>
                <p className="text-center text-sm text-muted-foreground">
                  Already have an account?{" "}
                  <button type="button" className="text-primary hover:underline" onClick={() => { setMode("login"); setAuthError(null); }}>
                    Sign in
                  </button>
                </p>
              </form>
            )}
          </CardContent>
        </Card>
      </div>
    </SiteShell>
  );
}