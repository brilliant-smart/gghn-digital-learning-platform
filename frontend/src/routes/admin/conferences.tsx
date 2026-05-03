import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Calendar, MapPin, Building, Users, UserPlus, Pencil, Trash2 } from "lucide-react";
import { conferenceApi, type ConferenceDto, type SpeakerDto, type SessionDto, type SponsorDto } from "@/api/conferences";
import { useAuth } from "@/lib/auth";
import { useNavigate } from "@tanstack/react-router";

export const Route = createFileRoute("/admin/conferences")({
  head: () => ({ meta: [{ title: "Conference Management | GGHN Admin" }] }),
  component: AdminConferencesPage,
});

function AdminConferencesPage() {
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [conferences, setConferences] = useState<ConferenceDto[]>([]);
  const [speakers, setSpeakers] = useState<SpeakerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<"conferences" | "speakers">("conferences");
  const [editingSpeaker, setEditingSpeaker] = useState<SpeakerDto | null>(null);
  const [newSpeaker, setNewSpeaker] = useState({ name: "", title: "", organization: "", bio: "" });
  const [addingSpeaker, setAddingSpeaker] = useState(false);

  useEffect(() => {
    if (!isAuthenticated || !user?.roles?.some((r: string) => r === "Admin" || r === "Editor")) {
      navigate({ to: "/dashboard", replace: true });
      return;
    }
    Promise.all([
      conferenceApi.getAll().catch(() => [] as ConferenceDto[]),
      conferenceApi.getSpeakers().catch(() => [] as SpeakerDto[]),
    ]).then(([c, s]) => {
      setConferences(c);
      setSpeakers(s);
    }).finally(() => setLoading(false));
  }, [isAuthenticated]);

  const handleDeleteConference = async (id: string) => {
    if (!confirm("Delete this conference?")) return;
    await conferenceApi.delete(id);
    setConferences((c) => c.filter((x) => x.id !== id));
  };

  const handleDeleteSpeaker = async (id: string) => {
    if (!confirm("Delete this speaker?")) return;
    await conferenceApi.deleteSpeaker(id);
    setSpeakers((s) => s.filter((x) => x.id !== id));
  };

  if (loading) return <SiteShell><div className="flex justify-center py-20"><p className="text-muted-foreground">Loading...</p></div></SiteShell>;

  return (
    <SiteShell>
      <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <h1 className="text-2xl font-bold tracking-tight mb-2">Conference Management</h1>
        <p className="text-muted-foreground text-sm mb-8">Manage conferences, speakers, and related content.</p>

        {/* Tabs */}
        <div className="flex gap-2 mb-6">
          {(["conferences", "speakers"] as const).map((t) => (
            <Button key={t} variant={activeTab === t ? "default" : "outline"} size="sm" onClick={() => setActiveTab(t)}>
              {t === "conferences" ? "Conferences" : "Speakers"}
            </Button>
          ))}
        </div>

        {activeTab === "conferences" && (
          <div className="space-y-4">
            {conferences.length === 0 ? (
              <Card><CardContent className="py-8 text-center text-muted-foreground">No conferences yet.</CardContent></Card>
            ) : (
              conferences.map((c) => (
                <Card key={c.id}>
                  <CardHeader className="flex-row items-start justify-between space-y-0">
                    <div>
                      <CardTitle className="text-lg">{c.title}</CardTitle>
                      <p className="text-sm text-muted-foreground mt-1">{c.theme}</p>
                    </div>
                    <div className="flex gap-2">
                      <Button variant="ghost" size="icon" title="Delete" onClick={() => handleDeleteConference(c.id)}>
                        <Trash2 className="h-4 w-4 text-destructive" />
                      </Button>
                    </div>
                  </CardHeader>
                  <CardContent>
                    <div className="flex flex-wrap gap-4 text-sm text-muted-foreground mb-4">
                      <span className="flex items-center gap-1"><Calendar className="h-3.5 w-3.5" /> {new Date(c.startDate).toLocaleDateString()} – {new Date(c.endDate).toLocaleDateString()}</span>
                      <span className="flex items-center gap-1"><MapPin className="h-3.5 w-3.5" /> {c.venue}</span>
                      <Badge variant={c.isArchived ? "secondary" : "default"}>{c.isArchived ? "Archived" : "Active"}</Badge>
                    </div>
                    <div className="grid grid-cols-3 gap-4 text-sm">
                      <div><span className="text-muted-foreground">Sessions: </span>{c.sessions?.length || 0}</div>
                      <div><span className="text-muted-foreground">Sponsors: </span>{c.sponsors?.length || 0}</div>
                      <div><span className="text-muted-foreground">Year: </span>{c.year}</div>
                    </div>
                  </CardContent>
                </Card>
              ))
            )}
          </div>
        )}

        {activeTab === "speakers" && (
          <div className="space-y-4">
            <div className="flex justify-end">
              <Button size="sm" onClick={() => setAddingSpeaker(!addingSpeaker)}>
                <UserPlus className="h-4 w-4 mr-1" /> Add Speaker
              </Button>
            </div>

            {addingSpeaker && (
              <Card>
                <CardContent className="pt-6 space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div><Label>Name</Label><Input value={newSpeaker.name} onChange={(e) => setNewSpeaker((s) => ({ ...s, name: e.target.value }))} /></div>
                    <div><Label>Title</Label><Input value={newSpeaker.title} onChange={(e) => setNewSpeaker((s) => ({ ...s, title: e.target.value }))} /></div>
                  </div>
                  <div><Label>Organization</Label><Input value={newSpeaker.organization} onChange={(e) => setNewSpeaker((s) => ({ ...s, organization: e.target.value }))} /></div>
                  <div><Label>Bio</Label><Input value={newSpeaker.bio} onChange={(e) => setNewSpeaker((s) => ({ ...s, bio: e.target.value }))} /></div>
                  <div className="flex gap-2 justify-end">
                    <Button variant="outline" size="sm" onClick={() => setAddingSpeaker(false)}>Cancel</Button>
                    <Button size="sm" onClick={async () => {
                      await conferenceApi.createSpeaker({ name: newSpeaker.name, title: newSpeaker.title, organization: newSpeaker.organization, bio: newSpeaker.bio || undefined });
                      const updated = await conferenceApi.getSpeakers();
                      setSpeakers(updated);
                      setAddingSpeaker(false);
                      setNewSpeaker({ name: "", title: "", organization: "", bio: "" });
                    }}>Save</Button>
                  </div>
                </CardContent>
              </Card>
            )}

            {speakers.map((s) => (
              <Card key={s.id}>
                <CardHeader className="flex-row items-start justify-between space-y-0">
                  <div>
                    <CardTitle className="text-base">{s.name}</CardTitle>
                    <p className="text-sm text-muted-foreground">{s.title} · {s.organization}</p>
                    {s.bio && <p className="text-xs text-muted-foreground mt-1 line-clamp-2">{s.bio}</p>}
                  </div>
                  <Button variant="ghost" size="icon" title="Delete" onClick={() => handleDeleteSpeaker(s.id)}>
                    <Trash2 className="h-4 w-4 text-destructive" />
                  </Button>
                </CardHeader>
              </Card>
            ))}
          </div>
        )}
      </div>
    </SiteShell>
  );
}
