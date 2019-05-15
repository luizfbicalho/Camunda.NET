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
namespace org.camunda.bpm.container.impl.jboss.deployment.marker
{

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessesXml = org.camunda.bpm.application.impl.metadata.spi.ProcessesXml;
	using ProcessesXmlWrapper = org.camunda.bpm.container.impl.jboss.util.ProcessesXmlWrapper;
	using ComponentDescription = org.jboss.@as.ee.component.ComponentDescription;
	using AttachmentKey = org.jboss.@as.server.deployment.AttachmentKey;
	using AttachmentList = org.jboss.@as.server.deployment.AttachmentList;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using AnnotationInstance = org.jboss.jandex.AnnotationInstance;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationAttachments
	{

	  private static readonly AttachmentKey<bool> MARKER = AttachmentKey.create(typeof(Boolean));
	  private static readonly AttachmentKey<bool> PART_OF_MARKER = AttachmentKey.create(typeof(Boolean));
	  private static readonly AttachmentKey<AttachmentList<ProcessesXmlWrapper>> PROCESSES_XML_LIST = AttachmentKey.createList(typeof(ProcessesXmlWrapper));
	  private static readonly AttachmentKey<ComponentDescription> PA_COMPONENT = AttachmentKey.create(typeof(ComponentDescription));
	  private static readonly AttachmentKey<AnnotationInstance> POST_DEPLOY_METHOD = AttachmentKey.create(typeof(AnnotationInstance));
	  private static readonly AttachmentKey<AnnotationInstance> PRE_UNDEPLOY_METHOD = AttachmentKey.create(typeof(AnnotationInstance));

	  /// <summary>
	  /// Attach the parsed ProcessesXml file to a deployment unit.
	  /// 
	  /// </summary>
	  public static void addProcessesXml(DeploymentUnit unit, ProcessesXmlWrapper processesXmlWrapper)
	  {
		unit.addToAttachmentList(PROCESSES_XML_LIST, processesXmlWrapper);
	  }

	  /// <summary>
	  /// Returns the attached <seealso cref="ProcessesXml"/> marker or null;
	  /// 
	  /// </summary>
	  public static IList<ProcessesXmlWrapper> getProcessesXmls(DeploymentUnit deploymentUnit)
	  {
		return deploymentUnit.getAttachmentList(PROCESSES_XML_LIST);
	  }

	  /// <summary>
	  /// marks a a <seealso cref="DeploymentUnit"/> as a process application 
	  /// </summary>
	  public static void mark(DeploymentUnit unit)
	  {
		unit.putAttachment(MARKER, true);
	  }

	  /// <summary>
	  /// marks a a <seealso cref="DeploymentUnit"/> as part of a process application 
	  /// </summary>
	  public static void markPartOfProcessApplication(DeploymentUnit unit)
	  {
		if (unit.Parent != null && unit.Parent != unit)
		{
		  unit.Parent.putAttachment(PART_OF_MARKER, true);
		}
	  }

	  /// <summary>
	  /// return true if the deployment unit is either itself a process 
	  /// application or part of a process application.
	  /// </summary>
	  public static bool isPartOfProcessApplication(DeploymentUnit unit)
	  {
		if (isProcessApplication(unit))
		{
		  return true;
		}
		if (unit.Parent != null && unit.Parent != unit)
		{
		  return unit.Parent.hasAttachment(PART_OF_MARKER);
		}
		return false;
	  }

	  /// <summary>
	  /// Returns true if the <seealso cref="DeploymentUnit"/> itself is a process application (carries a processes.xml)
	  /// 
	  /// </summary>
	  public static bool isProcessApplication(DeploymentUnit deploymentUnit)
	  {
		return deploymentUnit.hasAttachment(MARKER);
	  }

	  /// <summary>
	  /// Returns the <seealso cref="ComponentDescription"/> for the <seealso cref="AbstractProcessApplication"/> component
	  /// </summary>
	  public static ComponentDescription getProcessApplicationComponent(DeploymentUnit deploymentUnit)
	  {
		return deploymentUnit.getAttachment(PA_COMPONENT);
	  }

	  /// <summary>
	  /// Attach the <seealso cref="ComponentDescription"/> for the <seealso cref="AbstractProcessApplication"/> component   
	  /// </summary>
	  public static void attachProcessApplicationComponent(DeploymentUnit deploymentUnit, ComponentDescription componentDescription)
	  {
		deploymentUnit.putAttachment(PA_COMPONENT, componentDescription);
	  }

	  /// <summary>
	  /// Attach the <seealso cref="AnnotationInstance"/>s for the PostDeploy methods
	  /// </summary>
	  public static void attachPostDeployDescription(DeploymentUnit deploymentUnit, AnnotationInstance annotation)
	  {
		deploymentUnit.putAttachment(POST_DEPLOY_METHOD, annotation);
	  }

	  /// <summary>
	  /// Attach the <seealso cref="AnnotationInstance"/>s for the PreUndeploy methods
	  /// </summary>
	  public static void attachPreUndeployDescription(DeploymentUnit deploymentUnit, AnnotationInstance annotation)
	  {
		deploymentUnit.putAttachment(PRE_UNDEPLOY_METHOD, annotation);
	  }

	  /// <returns> the description of the PostDeploy method </returns>
	  public static AnnotationInstance getPostDeployDescription(DeploymentUnit deploymentUnit)
	  {
		return deploymentUnit.getAttachment(POST_DEPLOY_METHOD);
	  }

	  /// <returns> the description of the PreUndeploy method </returns>
	  public static AnnotationInstance getPreUndeployDescription(DeploymentUnit deploymentUnit)
	  {
		return deploymentUnit.getAttachment(PRE_UNDEPLOY_METHOD);
	  }

	  private ProcessApplicationAttachments()
	  {

	  }
	}

}