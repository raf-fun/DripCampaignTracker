import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { createCampaign } from '../services/api'

export default function CreateCampaign() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [form, setForm] = useState({
    name: '',
    goalTarget: '',
    cooldownDays: '',
    autoClose: false,
    marketerId: 1,
    managerId: 2
  })

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target
    setForm(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }))
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

    setLoading(true)
    setError(null)

    try {
      await createCampaign({
        name: form.name,
        goalTarget: parseInt(form.goalTarget),
        cooldownDays: parseInt(form.cooldownDays),
        autoClose: form.autoClose,
        marketerId: form.marketerId,
        managerId: form.managerId
      })
      navigate('/')
    } catch {
      setError('Failed to create campaign')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ padding: '2rem', maxWidth: '500px', margin: '0 auto' }}>
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