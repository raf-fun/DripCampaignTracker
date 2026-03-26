import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { createCampaign, getLeads } from '../services/api'

export default function CreateCampaign() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [leads, setLeads] = useState([])
  const [selectedLeads, setSelectedLeads] = useState([])
  const [form, setForm] = useState({
    name: '',
    goalTarget: '',
    cooldownDays: '',
    autoClose: false,
    marketerId: 1,
    managerId: 2
  })

  useEffect(() => {
    getLeads().then(res => setLeads(res.data))
  }, [])

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target
    setForm(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }))
  }

  const toggleLead = (leadId) => {
    setSelectedLeads(prev =>
      prev.includes(leadId)
        ? prev.filter(id => id !== leadId)
        : [...prev, leadId]
    )
  }

  const handleSubmit = async () => {
    if (!form.name || !form.goalTarget || !form.cooldownDays) {
      setError('Please fill in all fields')
      return
    }

    if (parseInt(form.goalTarget) < 10) {
      setError('Goal target must be at least 10 leads')
      return
    }

    if (parseInt(form.cooldownDays) < 2) {
      setError('Cooldown must be at least 2 days')
      return
    }

    if (selectedLeads.length < 10) {
      setError('You must select at least 10 leads')
      return
    }

    setLoading(true)
    setError(null)

    try {
      await createCampaign({
        name: form.name,
        goalTarget: parseInt(form.goalTarget),
        cooldownDays: parseInt(form.cooldownDays),
        autoClose: form.autoClose,
        marketerId: form.marketerId,
        managerId: form.managerId,
        leadIds: selectedLeads
      })
      navigate('/')
    } catch {
      setError('Failed to create campaign')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ padding: '2rem', maxWidth: '600px', margin: '0 auto' }}>
      <button onClick={() => navigate('/')}>← Back</button>

      <h1 style={{ marginTop: '1rem', marginBottom: '1.5rem' }}>New Campaign</h1>

      {error && <div style={{ color: 'red', marginBottom: '1rem' }}>{error}</div>}

      <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        <div>
          <label>Campaign Name</label>
          <input
            name="name"
            value={form.name}
            onChange={handleChange}
            placeholder="Enter campaign name"
            style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }}
          />
        </div>

        <div>
          <label>Goal Target (min 10)</label>
          <input
            name="goalTarget"
            type="number"
            value={form.goalTarget}
            onChange={handleChange}
            placeholder="e.g. 50"
            style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }}
          />
        </div>

        <div>
          <label>Cooldown Days (min 2)</label>
          <input
            name="cooldownDays"
            type="number"
            value={form.cooldownDays}
            onChange={handleChange}
            placeholder="e.g. 3"
            style={{ display: 'block', width: '100%', padding: '0.5rem', marginTop: '0.25rem' }}
          />
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <input
            name="autoClose"
            type="checkbox"
            checked={form.autoClose}
            onChange={handleChange}
          />
          <label>Auto Close when goal is reached</label>
        </div>

        <div>
          <label>Select Leads ({selectedLeads.length} selected — min 10)</label>
          <div style={{
            border: '1px solid #ccc',
            borderRadius: '8px',
            marginTop: '0.5rem',
            maxHeight: '250px',
            overflowY: 'auto'
          }}>
            {leads.map(lead => (
              <div
                key={lead.id}
                onClick={() => toggleLead(lead.id)}
                style={{
                  padding: '0.75rem 1rem',
                  cursor: 'pointer',
                  borderBottom: '1px solid #333',
                  background: selectedLeads.includes(lead.id) ? '#1a3a1a' : 'transparent',
                  display: 'flex',
                  justifyContent: 'space-between'
                }}
              >
                <span>{lead.name}</span>
                {selectedLeads.includes(lead.id) && <span>✓</span>}
              </div>
            ))}
          </div>
        </div>

        <button
          onClick={handleSubmit}
          disabled={loading}
          style={{ padding: '0.75rem', marginTop: '0.5rem' }}
        >
          {loading ? 'Creating...' : 'Create Campaign'}
        </button>
      </div>
    </div>
  )
}