using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.cdi.jsf
{


	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConversationScoped @Named("camundaTaskForm") public class TaskForm implements java.io.Serializable
	[Serializable]
	public class TaskForm
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static Logger log = Logger.getLogger(typeof(TaskForm).FullName);

	  private const long serialVersionUID = 1L;

	  protected internal string url;

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject protected org.camunda.bpm.engine.cdi.BusinessProcess businessProcess;
	  protected internal BusinessProcess businessProcess;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject protected org.camunda.bpm.engine.RepositoryService repositoryService;
	  protected internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject protected javax.enterprise.inject.Instance<javax.enterprise.context.Conversation> conversationInstance;
	  protected internal Instance<Conversation> conversationInstance;

	  /// @deprecated use <seealso cref="startTaskForm()"/> instead
	  /// 
	  /// <param name="taskId"> </param>
	  /// <param name="callbackUrl"> </param>
	  [Obsolete("use <seealso cref=\"startTaskForm()\"/> instead")]
	  public virtual void startTask(string taskId, string callbackUrl)
	  {
		if (string.ReferenceEquals(taskId, null) || string.ReferenceEquals(callbackUrl, null))
		{
		  if (FacesContext.CurrentInstance.Postback)
		  {
			// if this is an AJAX request ignore it, since we will receive multiple calls to this bean if it is added
			// as preRenderView event
			// see http://stackoverflow.com/questions/2830834/jsf-fevent-prerenderview-is-triggered-by-fajax-calls-and-partial-renders-some
			return;
		  }
		  // return it anyway but log an info message
		  log.log(Level.INFO, "Called startTask method without proper parameter (taskId='" + taskId + "'; callbackUrl='" + callbackUrl + "') even if it seems we are not called by an AJAX Postback. Are you using the camundaTaskForm bean correctly?");
		  return;
		}
		// Note that we always run in a conversation
		this.url = callbackUrl;
		businessProcess.startTask(taskId, true);
	  }

	  /// <summary>
	  /// Get taskId and callBackUrl from request and start a conversation
	  /// to start the form
	  /// 
	  /// </summary>
	  public virtual void startTaskForm()
	  {
		IDictionary<string, string> requestParameterMap = FacesContext.CurrentInstance.ExternalContext.RequestParameterMap;
		string taskId = requestParameterMap["taskId"];
		string callbackUrl = requestParameterMap["callbackUrl"];

		if (string.ReferenceEquals(taskId, null) || string.ReferenceEquals(callbackUrl, null))
		{
		  if (FacesContext.CurrentInstance.Postback)
		  {
			// if this is an AJAX request ignore it, since we will receive multiple calls to this bean if it is added
			// as preRenderView event
			// see http://stackoverflow.com/questions/2830834/jsf-fevent-prerenderview-is-triggered-by-fajax-calls-and-partial-renders-some
			return;
		  }
		  // return it anyway but log an info message
		  log.log(Level.INFO, "Called startTask method without proper parameter (taskId='" + taskId + "'; callbackUrl='" + callbackUrl + "') even if it seems we are not called by an AJAX Postback. Are you using the camundaTaskForm bean correctly?");
		  return;
		}
		// Note that we always run in a conversation
		this.url = callbackUrl;
		businessProcess.startTask(taskId, true);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completeTask() throws java.io.IOException
	  public virtual void completeTask()
	  {
		// the conversation is always ended on task completion (otherwise the
		// redirect will end up in an exception anyway!)
		businessProcess.completeTask(true);
		FacesContext.CurrentInstance.ExternalContext.redirect(url);
	  }

	  private void beginConversation()
	  {
		if (conversationInstance.get().Transient)
		{
		  conversationInstance.get().begin();
		}
	  }

	  /// @deprecated use <seealso cref="startProcessInstanceByIdForm()"/> instead
	  /// 
	  /// <param name="processDefinitionId"> </param>
	  /// <param name="callbackUrl"> </param>
	  [Obsolete("use <seealso cref=\"startProcessInstanceByIdForm()\"/> instead")]
	  public virtual void startProcessInstanceByIdForm(string processDefinitionId, string callbackUrl)
	  {
		this.url = callbackUrl;
		this.processDefinitionId = processDefinitionId;
		beginConversation();
	  }

	  /// <summary>
	  /// Get processDefinitionId and callbackUrl from request and start a conversation
	  /// to start the form
	  /// 
	  /// </summary>
	  public virtual void startProcessInstanceByIdForm()
	  {
		if (FacesContext.CurrentInstance.Postback)
		{
		  // if this is an AJAX request ignore it, since we will receive multiple calls to this bean if it is added
		  // as preRenderView event
		  // see http://stackoverflow.com/questions/2830834/jsf-fevent-prerenderview-is-triggered-by-fajax-calls-and-partial-renders-some
		  return;
		}

		IDictionary<string, string> requestParameterMap = FacesContext.CurrentInstance.ExternalContext.RequestParameterMap;
		string processDefinitionId = requestParameterMap["processDefinitionId"];
		string callbackUrl = requestParameterMap["callbackUrl"];
		this.url = callbackUrl;
		this.processDefinitionId = processDefinitionId;
		beginConversation();
	  }

	  /// @deprecated use <seealso cref="startProcessInstanceByKeyForm()"/> instead
	  /// 
	  /// <param name="processDefinitionKey"> </param>
	  /// <param name="callbackUrl"> </param>
	  [Obsolete("use <seealso cref=\"startProcessInstanceByKeyForm()\"/> instead")]
	  public virtual void startProcessInstanceByKeyForm(string processDefinitionKey, string callbackUrl)
	  {
		this.url = callbackUrl;
		this.processDefinitionKey = processDefinitionKey;
		beginConversation();
	  }

	  /// <summary>
	  /// Get processDefinitionKey and callbackUrl from request and start a conversation
	  /// to start the form
	  /// 
	  /// </summary>
	  public virtual void startProcessInstanceByKeyForm()
	  {
		if (FacesContext.CurrentInstance.Postback)
		{
		  // if this is an AJAX request ignore it, since we will receive multiple calls to this bean if it is added
		  // as preRenderView event
		  // see http://stackoverflow.com/questions/2830834/jsf-fevent-prerenderview-is-triggered-by-fajax-calls-and-partial-renders-some
		  return;
		}

		IDictionary<string, string> requestParameterMap = FacesContext.CurrentInstance.ExternalContext.RequestParameterMap;
		string processDefinitionKey = requestParameterMap["processDefinitionKey"];
		string callbackUrl = requestParameterMap["callbackUrl"];
		this.url = callbackUrl;
		this.processDefinitionKey = processDefinitionKey;
		beginConversation();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void completeProcessInstanceForm() throws java.io.IOException
	  public virtual void completeProcessInstanceForm()
	  {
		// start the process instance
		if (!string.ReferenceEquals(processDefinitionId, null))
		{
		  businessProcess.startProcessById(processDefinitionId);
		  processDefinitionId = null;
		}
		else
		{
		  businessProcess.startProcessByKey(processDefinitionKey);
		  processDefinitionKey = null;
		}

		// End the conversation
		conversationInstance.get().end();

		// and redirect
		FacesContext.CurrentInstance.ExternalContext.redirect(url);
	  }

	  public virtual ProcessDefinition ProcessDefinition
	  {
		  get
		  {
			// TODO cache result to avoid multiple queries within one page request
			if (!string.ReferenceEquals(processDefinitionId, null))
			{
			  return repositoryService.createProcessDefinitionQuery().processDefinitionId(processDefinitionId).singleResult();
			}
			else
			{
			  return repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).latestVersion().singleResult();
			}
		  }
	  }

	  public virtual string Url
	  {
		  get
		  {
			return url;
		  }
		  set
		  {
			this.url = value;
		  }
	  }

	}

}