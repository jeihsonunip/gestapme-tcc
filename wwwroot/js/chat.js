(function () {
  const historico = document.getElementById('chatHistorico');
  const form = document.getElementById('chatForm');
  const input = document.getElementById('chatInput');
  const btnEnviar = document.getElementById('chatEnviar');
  const tplUser = document.getElementById('tplBubbleUser');
  const tplBot = document.getElementById('tplBubbleBot');
  const tplErro = document.getElementById('tplBubbleErro');
  const tplDigitando = document.getElementById('tplDigitando');

  function renderBubble(template, texto) {
    const node = template.content.firstElementChild.cloneNode(true);
    node.querySelector('.gp-chat-text').textContent = texto;
    historico.appendChild(node);
    historico.scrollTop = historico.scrollHeight;
    return node;
  }

  function renderDigitando() {
    const node = tplDigitando.content.firstElementChild.cloneNode(true);
    historico.appendChild(node);
    historico.scrollTop = historico.scrollHeight;
    return node;
  }

  async function enviar(pergunta) {
    if (!pergunta.trim()) return;
    renderBubble(tplUser, pergunta);
    input.value = '';
    input.disabled = true; btnEnviar.disabled = true;

    const digitando = renderDigitando();

    try {
      const controller = new AbortController();
      const timeout = setTimeout(() => controller.abort(), 30000);

      const resp = await fetch('/api/assistente', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'same-origin',
        body: JSON.stringify({ pergunta }),
        signal: controller.signal
      });
      clearTimeout(timeout);
      digitando.remove();

      if (!resp.ok) {
        renderBubble(tplErro, 'Não consegui responder agora. Tente reformular.');
        return;
      }
      const data = await resp.json();
      renderBubble(tplBot, data.resposta || '(sem resposta)');
    } catch (err) {
      digitando.remove();
      renderBubble(tplErro, 'Tempo esgotado ou falha de conexão. Tente novamente.');
    } finally {
      input.disabled = false; btnEnviar.disabled = false;
      input.focus();
    }
  }

  form.addEventListener('submit', (e) => { e.preventDefault(); enviar(input.value); });

  input.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); enviar(input.value); }
  });

  document.querySelectorAll('[data-sugestao]').forEach((chip) => {
    chip.addEventListener('click', () => enviar(chip.dataset.sugestao));
  });
})();
