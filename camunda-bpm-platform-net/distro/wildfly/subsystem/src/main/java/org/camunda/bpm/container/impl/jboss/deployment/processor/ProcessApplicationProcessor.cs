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
namespace org.camunda.bpm.container.impl.jboss.deployment.processor
{

	using PostDeploy = org.camunda.bpm.application.PostDeploy;
	using PreUndeploy = org.camunda.bpm.application.PreUndeploy;
	using ProcessApplication = org.camunda.bpm.application.ProcessApplication;
	using ServletProcessApplication = org.camunda.bpm.application.impl.ServletProcessApplication;
	using ProcessApplicationAttachments = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments;
	using Attachments = org.jboss.@as.ee.component.Attachments;
	using ComponentDescription = org.jboss.@as.ee.component.ComponentDescription;
	using EEApplicationClasses = org.jboss.@as.ee.component.EEApplicationClasses;
	using EEModuleDescription = org.jboss.@as.ee.component.EEModuleDescription;
	using SessionBeanComponentDescription = org.jboss.@as.ejb3.component.session.SessionBeanComponentDescription;
	using DeploymentPhaseContext = org.jboss.@as.server.deployment.DeploymentPhaseContext;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using DeploymentUnitProcessingException = org.jboss.@as.server.deployment.DeploymentUnitProcessingException;
	using DeploymentUnitProcessor = org.jboss.@as.server.deployment.DeploymentUnitProcessor;
	using CompositeIndex = org.jboss.@as.server.deployment.annotation.CompositeIndex;
	using WarMetaData = org.jboss.@as.web.common.WarMetaData;
	using WebComponentDescription = org.jboss.@as.web.common.WebComponentDescription;
	using AnnotationInstance = org.jboss.jandex.AnnotationInstance;
	using ClassInfo = org.jboss.jandex.ClassInfo;
	using DotName = org.jboss.jandex.DotName;
	using JBossWebMetaData = org.jboss.metadata.web.jboss.JBossWebMetaData;
	using ListenerMetaData = org.jboss.metadata.web.spec.ListenerMetaData;


	/// <summary>
	/// <para>This processor detects a user-provided component annotated with the <seealso cref="ProcessApplication"/>-annotation.</para>
	/// 
	/// <para>If no such component is found but the deployment unit carries a META-INF/processes.xml file, a 
	/// Singleton Session Bean component is synthesized.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationProcessor : DeploymentUnitProcessor
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger log = Logger.getLogger(typeof(ProcessApplicationProcessor).FullName);

	  public const int PRIORITY = 0x2010; // after PARSE_WEB_MERGE_METADATA

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deploy(org.jboss.as.server.deployment.DeploymentPhaseContext phaseContext) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  public virtual void deploy(DeploymentPhaseContext phaseContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.DeploymentUnit deploymentUnit = phaseContext.getDeploymentUnit();
		DeploymentUnit deploymentUnit = phaseContext.DeploymentUnit;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.ee.component.EEModuleDescription eeModuleDescription = deploymentUnit.getAttachment(org.jboss.as.ee.component.Attachments.EE_MODULE_DESCRIPTION);
		EEModuleDescription eeModuleDescription = deploymentUnit.getAttachment(Attachments.EE_MODULE_DESCRIPTION);

		// must be EE Module
		if (eeModuleDescription == null)
		{
		  return;
		}

		// discover user-provided component
		ComponentDescription paComponent = detectExistingComponent(deploymentUnit);

		if (paComponent != null)
		{
		  log.log(Level.INFO, "Detected user-provided @" + typeof(ProcessApplication).Name + " component with name '" + paComponent.ComponentName + "'.");

		  // mark this to be a process application
		  ProcessApplicationAttachments.attachProcessApplicationComponent(deploymentUnit, paComponent);
		  ProcessApplicationAttachments.mark(deploymentUnit);
		  ProcessApplicationAttachments.markPartOfProcessApplication(deploymentUnit);
		}
	  }

	  /// <summary>
	  /// Detect an existing <seealso cref="ProcessApplication"/> component.  
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.jboss.as.ee.component.ComponentDescription detectExistingComponent(org.jboss.as.server.deployment.DeploymentUnit deploymentUnit) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  protected internal virtual ComponentDescription detectExistingComponent(DeploymentUnit deploymentUnit)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.ee.component.EEModuleDescription eeModuleDescription = deploymentUnit.getAttachment(org.jboss.as.ee.component.Attachments.EE_MODULE_DESCRIPTION);
		EEModuleDescription eeModuleDescription = deploymentUnit.getAttachment(Attachments.EE_MODULE_DESCRIPTION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.ee.component.EEApplicationClasses eeApplicationClasses = deploymentUnit.getAttachment(org.jboss.as.ee.component.Attachments.EE_APPLICATION_CLASSES_DESCRIPTION);
		EEApplicationClasses eeApplicationClasses = deploymentUnit.getAttachment(Attachments.EE_APPLICATION_CLASSES_DESCRIPTION);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.annotation.CompositeIndex compositeIndex = deploymentUnit.getAttachment(org.jboss.as.server.deployment.Attachments.COMPOSITE_ANNOTATION_INDEX);
		CompositeIndex compositeIndex = deploymentUnit.getAttachment(org.jboss.@as.server.deployment.Attachments.COMPOSITE_ANNOTATION_INDEX);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.web.common.WarMetaData warMetaData = deploymentUnit.getAttachment(org.jboss.as.web.common.WarMetaData.ATTACHMENT_KEY);
		WarMetaData warMetaData = deploymentUnit.getAttachment(WarMetaData.ATTACHMENT_KEY);

		// extract deployment metadata
		IList<AnnotationInstance> processApplicationAnnotations = null;
		IList<AnnotationInstance> postDeployAnnnotations = null;
		IList<AnnotationInstance> preUndeployAnnnotations = null;
		ISet<ClassInfo> servletProcessApplications = null;

		if (compositeIndex != null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  processApplicationAnnotations = compositeIndex.getAnnotations(DotName.createSimple(typeof(ProcessApplication).FullName));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  postDeployAnnnotations = compositeIndex.getAnnotations(DotName.createSimple(typeof(PostDeploy).FullName));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  preUndeployAnnnotations = compositeIndex.getAnnotations(DotName.createSimple(typeof(PreUndeploy).FullName));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  servletProcessApplications = compositeIndex.getAllKnownSubclasses(DotName.createSimple(typeof(ServletProcessApplication).FullName));
		}
		else
		{
		  return null;
		}

		if (processApplicationAnnotations.Count == 0)
		{
		  // no pa found, this is not a process application deployment.
		  return null;

		}
		else if (processApplicationAnnotations.Count > 1)
		{
		  // found multiple PAs -> unsupported.      
		  throw new DeploymentUnitProcessingException("Detected multiple classes annotated with @" + typeof(ProcessApplication).Name + ". A deployment must only provide a single @" + typeof(ProcessApplication).Name + " class.");

		}
		else
		{
		  // found single PA

		  AnnotationInstance annotationInstance = processApplicationAnnotations[0];
		  ClassInfo paClassInfo = (ClassInfo) annotationInstance.target();
		  string paClassName = paClassInfo.name().ToString();

		  ComponentDescription paComponent = null;

		  // it can either be a Servlet Process Application or a Singleton Session Bean Component or
		  if (servletProcessApplications.Contains(paClassInfo))
		  {

			// Servlet Process Applications can only be deployed inside Web Applications
			if (warMetaData == null)
			{
			  throw new DeploymentUnitProcessingException("@ProcessApplication class is a ServletProcessApplication but deployment is not a Web Application.");
			}

			// check whether it's already a servlet context listener:
			JBossWebMetaData mergedJBossWebMetaData = warMetaData.MergedJBossWebMetaData;
			IList<ListenerMetaData> listeners = mergedJBossWebMetaData.Listeners;
			if (listeners == null)
			{
			  listeners = new List<ListenerMetaData>();
			  mergedJBossWebMetaData.Listeners = listeners;
			}

			bool isListener = false;
			foreach (ListenerMetaData listenerMetaData in listeners)
			{
			  if (listenerMetaData.ListenerClass.Equals(paClassInfo.name().ToString()))
			  {
				isListener = true;
			  }
			}

			if (!isListener)
			{
			  // register as Servlet Context Listener
			  ListenerMetaData listener = new ListenerMetaData();
			  listener.ListenerClass = paClassName;
			  listeners.Add(listener);

			  // synthesize WebComponent
			  WebComponentDescription paWebComponent = new WebComponentDescription(paClassName, paClassName, eeModuleDescription, deploymentUnit.ServiceName, eeApplicationClasses);

			  eeModuleDescription.addComponent(paWebComponent);

			  deploymentUnit.addToAttachmentList(WebComponentDescription.WEB_COMPONENTS, paWebComponent.StartServiceName);

			  paComponent = paWebComponent;

			}
			else
			{
			  // lookup the existing component          
			  paComponent = eeModuleDescription.getComponentsByClassName(paClassName).get(0);
			}

			// deactivate sci


		  }
		  else
		  {

			// if its not a ServletProcessApplication it must be a session bean component

			IList<ComponentDescription> componentsByClassName = eeModuleDescription.getComponentsByClassName(paClassName);

			if (componentsByClassName.Count > 0 && (componentsByClassName[0] is SessionBeanComponentDescription))
			{
			  paComponent = componentsByClassName[0];

			}
			else
			{
			  throw new DeploymentUnitProcessingException("Class " + paClassName + " is annotated with @" + typeof(ProcessApplication).Name + " but is neither a ServletProcessApplication nor an EJB Session Bean Component.");

			}

		  }

		  // attach additional metadata to the deployment unit

		  if (postDeployAnnnotations.Count > 0)
		  {
			if (postDeployAnnnotations.Count == 1)
			{
			  ProcessApplicationAttachments.attachPostDeployDescription(deploymentUnit, postDeployAnnnotations[0]);
			}
			else
			{
			  throw new DeploymentUnitProcessingException("There can only be a single method annotated with @PostDeploy. Found [" + postDeployAnnnotations + "]");
			}
		  }

		  if (preUndeployAnnnotations.Count > 0)
		  {
			if (preUndeployAnnnotations.Count == 1)
			{
			  ProcessApplicationAttachments.attachPreUndeployDescription(deploymentUnit, preUndeployAnnnotations[0]);
			}
			else
			{
			  throw new DeploymentUnitProcessingException("There can only be a single method annotated with @PreUndeploy. Found [" + preUndeployAnnnotations + "]");
			}
		  }

		  return paComponent;
		}
	  }

	  public override void undeploy(DeploymentUnit context)
	  {

	  }

	}

}