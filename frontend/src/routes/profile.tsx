import { createFileRoute, Link } from "@tanstack/react-router";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { useAuth } from "@/lib/auth";
import { authApi } from "@/api/auth";
import {
  Mail,
  Building2,
  Briefcase,
  Globe,
  Shield,
  User,
  Pencil,
  Check,
  X,
  LogOut,
  Crown,
  GraduationCap,
} from "lucide-react";
import { useState } from "react";

export const Route = createFileRoute("/profile")({
  head: () => ({
    meta: [
      { title: "Profile | GGHN Digital Learning" },
      { name: "description", content: "Your GGHN profile and account settings." },
    ],
  }),
  component: ProfilePage,
});

function ProfilePage() {
  const { user, isAuthenticated, logout } = useAuth();
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);

  const [firstName, setFirstName] = useState(user?.firstName || "");
  const [lastName, setLastName] = useState(user?.lastName || "");
  const [organization, setOrganization] = useState(user?.organization || "");
  const [jobTitle, setJobTitle] = useState(user?.jobTitle || "");
  const [country, setCountry] = useState(user?.country || "");

  if (!isAuthenticated || !user) {
    return (
      <SiteShell>
        <div className="flex min-h-[60vh] items-center justify-center px-4">
          <Card className="w-full max-w-md text-center">
            <CardHeader>
              <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-muted">
                <User className="h-8 w-8 text-muted-foreground" />
              </div>
              <CardTitle className="mt-4">Sign in required</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="mb-4 text-sm text-muted-foreground">
                You need to be signed in to view your profile.
              </p>
              <Link to="/auth">
                <Button className="w-full">Sign in</Button>
              </Link>
            </CardContent>
          </Card>
        </div>
      </SiteShell>
    );
  }

  const initials = `${user.firstName[0] || ""}${user.lastName[0] || ""}`;

  const tierConfig: Record<string, { label: string; icon: React.ElementType; color: string }> = {
    Free: { label: "Free", icon: User, color: "bg-muted text-muted-foreground" },
    Member: { label: "Member", icon: GraduationCap, color: "bg-primary-soft text-primary" },
    Institutional: { label: "Institutional", icon: Crown, color: "bg-secondary-soft text-secondary" },
  };

  const tier = tierConfig[user.membershipTier] || tierConfig.Free;
  const TierIcon = tier.icon;

  const handleSave = async () => {
    setSaving(true);
    try {
      const updated = await authApi.updateProfile({
        firstName,
        lastName,
        organization: organization || undefined,
        jobTitle: jobTitle || undefined,
        country: country || undefined,
      });
      // Optimistically update the auth context
      Object.assign(user, updated);
      setEditing(false);
    } catch {
      // Stay in edit mode on error
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    setFirstName(user.firstName);
    setLastName(user.lastName);
    setOrganization(user.organization || "");
    setJobTitle(user.jobTitle || "");
    setCountry(user.country || "");
    setEditing(false);
  };

  return (
    <SiteShell>
      {/* Hero banner */}
      <section
        className="relative overflow-hidden"
        style={{ backgroundImage: "var(--gradient-hero)" }}
      >
        <div className="pointer-events-none absolute -right-24 -top-24 h-64 w-64 rounded-full bg-white/10 blur-3xl" />
        <div className="relative mx-auto max-w-4xl px-4 py-12 sm:px-6 lg:px-8">
          <div className="flex items-center gap-5">
            <div className="flex h-20 w-20 items-center justify-center rounded-full border-3 border-white/30 bg-white/15 text-2xl font-bold text-white shadow-lg backdrop-blur-sm">
              {initials}
            </div>
            <div>
              <h1 className="text-2xl font-bold tracking-tight text-white sm:text-3xl">
                {user.firstName} {user.lastName}
              </h1>
              <p className="mt-1 text-sm text-white/70">{user.email}</p>
              <div className="mt-2 flex flex-wrap items-center gap-2">
                <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-semibold ${tier.color}`}>
                  <TierIcon className="h-3 w-3" />
                  {tier.label}
                </span>
                {user.roles.map((role) => (
                  <Badge key={role} variant="outline" className="border-white/30 bg-white/10 text-white">
                    {role}
                  </Badge>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      <div className="mx-auto max-w-4xl px-4 py-10 sm:px-6 lg:px-8">
        {/* Action bar */}
        <div className="mb-6 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-foreground">Profile details</h2>
          {!editing ? (
            <Button variant="outline" size="sm" onClick={() => setEditing(true)}>
              <Pencil className="mr-1.5 h-3.5 w-3.5" /> Edit profile
            </Button>
          ) : (
            <div className="flex gap-2">
              <Button size="sm" onClick={handleSave} disabled={saving}>
                <Check className="mr-1.5 h-3.5 w-3.5" /> {saving ? "Saving..." : "Save changes"}
              </Button>
              <Button variant="outline" size="sm" onClick={handleCancel}>
                <X className="mr-1.5 h-3.5 w-3.5" /> Cancel
              </Button>
            </div>
          )}
        </div>

        {/* Personal information */}
        <Card>
          <CardHeader className="pb-4">
            <CardTitle className="text-base">Personal information</CardTitle>
          </CardHeader>
          <CardContent>
            {editing ? (
              <div className="grid gap-5 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="firstName">First name</Label>
                  <Input id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="lastName">Last name</Label>
                  <Input id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="organization">Organization</Label>
                  <Input id="organization" value={organization} onChange={(e) => setOrganization(e.target.value)} placeholder="Your organization" />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="jobTitle">Job title</Label>
                  <Input id="jobTitle" value={jobTitle} onChange={(e) => setJobTitle(e.target.value)} placeholder="Your role" />
                </div>
                <div className="space-y-2 sm:col-span-2">
                  <Label htmlFor="country">Country</Label>
                  <Input id="country" value={country} onChange={(e) => setCountry(e.target.value)} placeholder="Your country" />
                </div>
              </div>
            ) : (
              <div className="grid gap-4 sm:grid-cols-2">
                <DetailRow icon={User} label="Full name" value={`${user.firstName} ${user.lastName}`} />
                <DetailRow icon={Mail} label="Email" value={user.email} />
                {user.organization && <DetailRow icon={Building2} label="Organization" value={user.organization} />}
                {user.jobTitle && <DetailRow icon={Briefcase} label="Job title" value={user.jobTitle} />}
                {user.country && <DetailRow icon={Globe} label="Country" value={user.country} />}
              </div>
            )}
          </CardContent>
        </Card>

        {/* Account & membership */}
        <Card className="mt-6">
          <CardHeader className="pb-4">
            <CardTitle className="text-base">Account & membership</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow icon={Shield} label="Membership tier" value={user.membershipTier} />
            <DetailRow icon={Shield} label="Roles" value={user.roles.join(", ")} />
            <Separator />
            <div className="rounded-lg border border-border bg-muted/30 p-4">
              <div className="flex items-start gap-3">
                <TierIcon className="mt-0.5 h-5 w-5 text-primary" />
                <div>
                  <p className="text-sm font-medium text-foreground">
                    {tier.label} tier
                  </p>
                  <p className="mt-1 text-xs text-muted-foreground">
                    {user.membershipTier === "Institutional"
                      ? "Full access to all content including institutional-grade resources, courses, and templates."
                      : user.membershipTier === "Member"
                        ? "Access to member-tier courses, templates, and resources. Upgrade to Institutional for full access."
                        : "Access to free courses and resources. Upgrade to Member or Institutional for premium content."}
                  </p>
                  {user.membershipTier !== "Institutional" && (
                    <Button variant="outline" size="sm" className="mt-3">
                      Upgrade membership
                    </Button>
                  )}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Danger zone */}
        <Card className="mt-6 border-destructive/20">
          <CardHeader className="pb-4">
            <CardTitle className="text-base text-destructive">Account</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-foreground">Sign out</p>
                <p className="text-xs text-muted-foreground">Sign out of your account on this device.</p>
              </div>
              <Button variant="outline" size="sm" onClick={logout} className="gap-1.5 text-destructive hover:bg-destructive/10 hover:text-destructive">
                <LogOut className="h-3.5 w-3.5" /> Sign out
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </SiteShell>
  );
}

function DetailRow({ icon: Icon, label, value }: { icon: React.ElementType; label: string; value: string }) {
  return (
    <div className="flex items-center gap-3">
      <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted">
        <Icon className="h-4 w-4 text-muted-foreground" />
      </div>
      <div className="min-w-0">
        <p className="text-[11px] font-medium uppercase tracking-wider text-muted-foreground">{label}</p>
        <p className="truncate text-sm font-medium text-foreground">{value}</p>
      </div>
    </div>
  );
}