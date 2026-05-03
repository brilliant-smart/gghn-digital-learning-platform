import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Badge } from "@/components/ui/badge";
import { Check, ChevronLeft, ChevronRight, Loader2 } from "lucide-react";
import { conferenceApi, type CreateRegistrationRequest } from "@/api/conferences";

interface Props {
  conferenceId: string;
  conferenceTitle: string;
  open: boolean;
  onClose: () => void;
}

const REGISTRATION_TYPES = ["General", "Speaker", "Sponsor", "VIP", "Media", "Scholarship"];
const STEPS = ["Type", "Personal", "Professional", "Additional", "Confirm"];
const COUNTRIES = ["Nigeria", "Ghana", "Kenya", "South Africa", "Uganda", "Tanzania", "Ethiopia", "United States", "United Kingdom", "Other"];

export function ConferenceRegistrationModal({ conferenceId, conferenceTitle, open, onClose }: Props) {
  const [step, setStep] = useState(0);
  const [submitting, setSubmitting] = useState(false);
  const [done, setDone] = useState(false);
  const [error, setError] = useState("");
  const [form, setForm] = useState<CreateRegistrationRequest>({
    conferenceId,
    firstName: "",
    lastName: "",
    email: "",
    organization: "",
    jobTitle: "",
    country: "",
    phoneNumber: "",
    registrationType: "General",
    dietaryRestrictions: "",
    accessibilityNeeds: "",
    specialRequests: "",
  });

  if (!open) return null;

  const update = (field: keyof CreateRegistrationRequest, value: string) =>
    setForm((f) => ({ ...f, [field]: value }));

  const canNext = () => {
    if (step === 0) return !!form.registrationType;
    if (step === 1) return form.firstName && form.lastName && form.email && form.country;
    if (step === 2) return form.organization && form.jobTitle;
    return true;
  };

  const handleSubmit = async () => {
    setSubmitting(true);
    setError("");
    try {
      await conferenceApi.register(form);
      setDone(true);
    } catch {
      setError("Something went wrong. Please try again.");
    } finally {
      setSubmitting(false);
    }
  };

  if (done) {
    return (
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
        <Card className="w-full max-w-md text-center">
          <CardContent className="pt-12 pb-8">
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-green-100">
              <Check className="h-8 w-8 text-green-600" />
            </div>
            <h3 className="mt-6 text-xl font-bold">Registration Submitted</h3>
            <p className="mt-2 text-muted-foreground">
              Your registration for {conferenceTitle} has been submitted for review. You will receive a confirmation email once approved.
            </p>
            <Button className="mt-6" onClick={onClose}>Close</Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <CardHeader>
          <CardTitle>Register for {conferenceTitle}</CardTitle>
          <CardDescription>Complete the form below. Your registration will be reviewed by our team.</CardDescription>

          {/* Step progress */}
          <div className="mt-4 flex items-center gap-1">
            {STEPS.map((s, i) => (
              <div key={s} className="flex items-center gap-1 flex-1 last:flex-[0_0_auto]">
                <button
                  onClick={() => i < step && setStep(i)}
                  className={`flex items-center justify-center h-7 px-2 rounded-full text-xs font-medium transition-colors ${
                    i < step
                      ? "bg-primary text-primary-foreground cursor-pointer"
                      : i === step
                      ? "bg-primary text-primary-foreground"
                      : "bg-muted text-muted-foreground"
                  }`}
                >
                  {i < step ? <Check className="h-3 w-3" /> : i + 1}
                </button>
                {i < STEPS.length - 1 && <div className={`flex-1 h-px ${i < step ? "bg-primary" : "bg-border"}`} />}
              </div>
            ))}
          </div>
        </CardHeader>
        <CardContent>
          {error && <p className="mb-4 text-sm text-destructive">{error}</p>}

          {/* Step 0: Registration Type */}
          {step === 0 && (
            <div>
              <Label className="text-sm font-medium mb-3 block">Select your registration type</Label>
              <div className="grid grid-cols-2 gap-2">
                {REGISTRATION_TYPES.map((t) => (
                  <button
                    key={t}
                    onClick={() => update("registrationType", t)}
                    className={`p-3 rounded-lg border text-left text-sm transition-colors ${
                      form.registrationType === t
                        ? "border-primary bg-primary-soft text-primary"
                        : "border-border hover:border-primary/50"
                    }`}
                  >
                    {t}
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Step 1: Personal Details */}
          {step === 1 && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>First Name *</Label>
                  <Input value={form.firstName} onChange={(e) => update("firstName", e.target.value)} placeholder="John" />
                </div>
                <div>
                  <Label>Last Name *</Label>
                  <Input value={form.lastName} onChange={(e) => update("lastName", e.target.value)} placeholder="Doe" />
                </div>
              </div>
              <div>
                <Label>Email *</Label>
                <Input type="email" value={form.email} onChange={(e) => update("email", e.target.value)} placeholder="john@example.org" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Country *</Label>
                  <Select value={form.country} onValueChange={(v) => update("country", v)}>
                    <SelectTrigger><SelectValue placeholder="Select..." /></SelectTrigger>
                    <SelectContent>
                      {COUNTRIES.map((c) => <SelectItem key={c} value={c}>{c}</SelectItem>)}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label>Phone</Label>
                  <Input value={form.phoneNumber || ""} onChange={(e) => update("phoneNumber", e.target.value)} placeholder="+234..." />
                </div>
              </div>
            </div>
          )}

          {/* Step 2: Professional Details */}
          {step === 2 && (
            <div className="space-y-4">
              <div>
                <Label>Organization *</Label>
                <Input value={form.organization} onChange={(e) => update("organization", e.target.value)} placeholder="Your organization" />
              </div>
              <div>
                <Label>Job Title *</Label>
                <Input value={form.jobTitle} onChange={(e) => update("jobTitle", e.target.value)} placeholder="Program Manager" />
              </div>
            </div>
          )}

          {/* Step 3: Additional Info */}
          {step === 3 && (
            <div className="space-y-4">
              <div>
                <Label>Dietary Restrictions</Label>
                <Input value={form.dietaryRestrictions || ""} onChange={(e) => update("dietaryRestrictions", e.target.value)} placeholder="Vegetarian, halal, allergies..." />
              </div>
              <div>
                <Label>Accessibility Needs</Label>
                <Input value={form.accessibilityNeeds || ""} onChange={(e) => update("accessibilityNeeds", e.target.value)} placeholder="Wheelchair access, sign language..." />
              </div>
              <div>
                <Label>Special Requests</Label>
                <Textarea value={form.specialRequests || ""} onChange={(e) => update("specialRequests", e.target.value)} placeholder="Any additional requirements or notes" rows={3} />
              </div>
            </div>
          )}

          {/* Step 4: Review & Confirm */}
          {step === 4 && (
            <div className="space-y-4">
              <h4 className="font-semibold">Review Your Registration</h4>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div><span className="text-muted-foreground">Type</span><p className="font-medium">{form.registrationType}</p></div>
                <div><span className="text-muted-foreground">Name</span><p className="font-medium">{form.firstName} {form.lastName}</p></div>
                <div><span className="text-muted-foreground">Email</span><p className="font-medium">{form.email}</p></div>
                <div><span className="text-muted-foreground">Country</span><p className="font-medium">{form.country}</p></div>
                <div><span className="text-muted-foreground">Organization</span><p className="font-medium">{form.organization}</p></div>
                <div><span className="text-muted-foreground">Job Title</span><p className="font-medium">{form.jobTitle}</p></div>
              </div>
              {form.dietaryRestrictions && <p className="text-sm"><span className="text-muted-foreground">Dietary: </span>{form.dietaryRestrictions}</p>}
              {form.accessibilityNeeds && <p className="text-sm"><span className="text-muted-foreground">Accessibility: </span>{form.accessibilityNeeds}</p>}
            </div>
          )}

          {/* Navigation buttons */}
          <div className="mt-8 flex items-center justify-between">
            <Button variant="outline" size="sm" disabled={step === 0} onClick={() => setStep((s) => s - 1)}>
              <ChevronLeft className="h-4 w-4 mr-1" /> Back
            </Button>
            <div className="flex items-center gap-2">
              <Button variant="ghost" size="sm" onClick={onClose}>Cancel</Button>
              {step < 4 ? (
                <Button size="sm" disabled={!canNext()} onClick={() => setStep((s) => s + 1)}>
                  Next <ChevronRight className="h-4 w-4 ml-1" />
                </Button>
              ) : (
                <Button size="sm" disabled={submitting} onClick={handleSubmit}>
                  {submitting && <Loader2 className="h-4 w-4 mr-1 animate-spin" />}
                  Submit Registration
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
